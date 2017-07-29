using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    public GameObject objectPlayer;
    private CircleCollider2D colliderPlayer;
    public float playerMoveSpeed = 5f;
    public Vector2 playerBounds = Vector2.one * 6f;

    public float energyCurrent = 1f;
    public float energyDecreaseRate = 0.01f;
    public float energyRecoverOnItemPickup = 0.4f;
    public Image imageEnergyGauge;

    public float stageMoveSpeedCurrent = 0f;
    public float stageMoveSpeedAcceleration = 2.5f;
    public float stageMoveSpeedMaximum = 6f;
    public float stageMoveSpeedMaximumIncreaseRate = 0.02f;
    public float stageMoveSpeedMaximumMaximum = 10f;
    public float stageDistanceMoved = 0f;

    public float interactableEnergyColRadius = 1f;
    public float interactableObstacleSmallColRadius = 1f;
    public float interactableObstacleSmallEnergyDecrease = 0.05f;

    [HideInInspector] public bool isGameOver = false;

    private GameInteractableSpawner[] arrayInteractableSpawner;

    void Awake()
    {
        control = this;
        colliderPlayer = objectPlayer.GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        arrayInteractableSpawner = FindObjectsOfType<GameInteractableSpawner>();
    }

    void Update()
    {
        // Do nothing if game has ended.
        if (isGameOver)
        {
            return;
        }

        // Increase speed if player has energy. Otherwise, decrease it.
        if (energyCurrent > 0f)
        {
            stageMoveSpeedCurrent = Mathf.Clamp(stageMoveSpeedCurrent + (stageMoveSpeedAcceleration * Time.deltaTime), 0f, stageMoveSpeedMaximum);
        }
        else
        {
            stageMoveSpeedCurrent = Mathf.Clamp(stageMoveSpeedCurrent - (stageMoveSpeedAcceleration * Time.deltaTime), 0f, stageMoveSpeedMaximum);
        }

        // As long as player has energy or speed, player can move.
        if (energyCurrent > Mathf.Epsilon || stageMoveSpeedCurrent > Mathf.Epsilon)
        {
            float moveX = Input.GetAxisRaw("Horizontal") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum;
            float moveY = Input.GetAxisRaw("Vertical") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum;

            Vector3 playerPos = objectPlayer.transform.localPosition;
            playerPos.x = Mathf.Clamp(playerPos.x + moveX, -playerBounds.x, playerBounds.x);
            playerPos.y = Mathf.Clamp(playerPos.y + moveY, -playerBounds.y, playerBounds.y);
            objectPlayer.transform.localPosition = playerPos;

            stageDistanceMoved += stageMoveSpeedCurrent * Time.deltaTime;

            if (stageMoveSpeedCurrent > stageMoveSpeedMaximum - 0.001f)
            {
                stageMoveSpeedMaximum += stageMoveSpeedMaximumIncreaseRate * Time.deltaTime;
                if (stageMoveSpeedMaximum > stageMoveSpeedMaximumMaximum)
                {
                    stageMoveSpeedMaximum = stageMoveSpeedMaximumMaximum;
                }
            }
        }
        // Otherwise, end the game.
        else if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("GAME OVER!!!");
            imageEnergyGauge.fillAmount = 0f;
            return;
        }

        // Energy decreasing.
        energyCurrent = Mathf.Clamp01(energyCurrent - (energyDecreaseRate * Time.deltaTime));
        imageEnergyGauge.fillAmount = energyCurrent;
    }

    void LateUpdate()
    {
        // Check each spawner for each interactable object list.
        foreach (GameInteractableSpawner x in arrayInteractableSpawner)
        {
            foreach (GameInteractable y in x.listInteractable)
            {
                float dist = Vector3.Distance(objectPlayer.transform.position, y.transform.position);

                switch (y.mInteractableType)
                {
                    // Energy restore
                    case GameInteractable.InteractableType.EnergyRestore:
                        if (dist < interactableEnergyColRadius)
                        {
                            Destroy(y.gameObject);
                            energyCurrent = Mathf.Clamp01(energyCurrent + energyRecoverOnItemPickup);
                        }
                        break;
                    // Small obstacle
                    case GameInteractable.InteractableType.ObstacleSmall:
                        if (dist < interactableObstacleSmallColRadius)
                        {
                            if (!y.isInteracted)
                            {
                                y.isInteracted = true;
                                stageMoveSpeedCurrent = 0f;
                                energyCurrent -= interactableObstacleSmallEnergyDecrease;
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning("This interactable type has not been implemented. InteractableType = " + y.mInteractableType.ToString());
                        break;
                }
            }
        }
    }
}
