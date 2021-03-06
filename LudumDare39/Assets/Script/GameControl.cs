﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    public GameObject objectPlayer;
    public Animator animatorPlayer;
    private CircleCollider2D colliderPlayer;
    public float playerMoveSpeed = 5f;
    public Vector2 playerBounds = Vector2.one * 6f;
    public ParticleSystem particleSystemPlayer;

    public GameObject objectHelp;
    public GameObject objectPause;
    public Animator animatorResult;

    public float energyCurrent = 1f;
    public float energyDecreaseRate = 0.01f;
    public float energyRecoverOnItemPickup = 0.4f;
    public RectTransform imageEnergyGauge;
    private Vector2 sizeEnergyGaugeHeight;

    public float stageMoveSpeedCurrent = 0f;
    public float stageMoveSpeedAcceleration = 2.5f;
    public float stageMoveSpeedMaximum = 6f;
    public float stageMoveSpeedMaximumIncreaseRate = 0.02f;
    public float stageMoveSpeedMaximumMaximum = 10f;
    public float stageDistanceMoved = 0f;
    public Text[] textDistance;

    public float interactableEnergyColRadius = 1f;
    public float interactableObstacleSmallColRadius = 1f;
    public float interactableObstacleSmallEnergyDecrease = 0.05f;
    public float interactableObstacleLargeColRadius = 2f;
    public float interactableObstacleLargeEnergyDecrease = 0.05f;

    public float invincibilityPeriodCurrent = 0f;
    public float invincibilityPeriodOnHitObstacleSmall = 0.5f;
    public float invincibilityPeriodOnHitObstacleLarge = 2f;

    public GameObject[] objectDisableOnGameOver;
    public ParticleSystem[] particleSystemStars;

    public AudioSource audioSourceSound;
    public AudioSource audioSourceMusic;
    public AudioClip clipDrink;
    public AudioClip clipHit;

    [HideInInspector] public bool isGameOver = false;
    private bool isPaused = false;
    private float pauseCooldown = 0f;

    private GameInteractableSpawner[] arrayInteractableSpawner;

    void Awake()
    {
        control = this;
        colliderPlayer = objectPlayer.GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        animatorResult.gameObject.SetActive(false);
        objectPause.SetActive(false);
        arrayInteractableSpawner = FindObjectsOfType<GameInteractableSpawner>();

        sizeEnergyGaugeHeight = imageEnergyGauge.sizeDelta;
        stageMoveSpeedCurrent = stageMoveSpeedMaximum * 0.5f;

        if (particleSystemPlayer != null)
        {
            particleSystemPlayer.Play();
        }
    }

    void Update()
    {
        // Do nothing if game has ended.
        if (isGameOver)
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) && pauseCooldown < 0f)
        {
            isPaused = !isPaused;

            objectPause.SetActive(isPaused);
            if (isPaused)
            {
                Time.timeScale = 0f;
                pauseCooldown = 0.5f;
            }
            else
            {
                Time.timeScale = 1f;
                pauseCooldown = 3f;
            }
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
            float moveX = Input.GetAxisRaw("Horizontal") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum * Time.deltaTime;
            float moveY = Input.GetAxisRaw("Vertical") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum * Time.deltaTime;

            Vector3 playerPos = objectPlayer.transform.localPosition;
            playerPos.x = Mathf.Clamp(playerPos.x + moveX, -playerBounds.x, playerBounds.x);
            playerPos.y = Mathf.Clamp(playerPos.y + moveY, -playerBounds.y, playerBounds.y);
            objectPlayer.transform.localPosition = playerPos;

            stageDistanceMoved += stageMoveSpeedCurrent * Time.deltaTime;
            objectHelp.SetActive(stageDistanceMoved < 100f);
            foreach (Text x in textDistance)
            {
                x.text = stageDistanceMoved.ToString("f1") + " m";
            }

            if (stageMoveSpeedCurrent > stageMoveSpeedMaximum - 0.001f)
            {
                stageMoveSpeedMaximum += stageMoveSpeedMaximumIncreaseRate * Time.deltaTime;
                if (stageMoveSpeedMaximum > stageMoveSpeedMaximumMaximum)
                {
                    stageMoveSpeedMaximum = stageMoveSpeedMaximumMaximum;
                }
            }

            if (energyCurrent < 0.0001f)
            {
                audioSourceMusic.pitch = stageMoveSpeedCurrent / stageMoveSpeedMaximum;
                if (particleSystemPlayer.isPlaying)
                {
                    particleSystemPlayer.Stop();
                }
            }
            else if (energyCurrent > 0.0001f && !particleSystemPlayer.isPlaying)
            {
                particleSystemPlayer.Play();
                audioSourceMusic.pitch = 1f;
            }

            foreach (ParticleSystem x in particleSystemStars)
            {
                x.playbackSpeed = 1f * stageMoveSpeedCurrent / stageMoveSpeedMaximum;
            }

            /*
            if (particleSystemPlayer != null)
            {
                particleSystemPlayer.playbackSpeed = 1f * stageMoveSpeedCurrent / stageMoveSpeedMaximum;
            }
            */
        }
        // Otherwise, end the game.
        else if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("GAME OVER!!!");
            //imageEnergyGauge.fillAmount = 0f;
            objectHelp.SetActive(false);
            animatorResult.gameObject.SetActive(true);
			audioSourceMusic.Stop ();

            if (particleSystemPlayer != null)
            {
                particleSystemPlayer.Stop();
            }
            foreach (ParticleSystem x in particleSystemStars)
            {
                x.Stop();
            }
            foreach (GameObject x in objectDisableOnGameOver)
            {
                x.SetActive(false);
            }
            return;
        }

        // Energy decreasing.
        energyCurrent = Mathf.Clamp01(energyCurrent - (energyDecreaseRate * Time.deltaTime));
        //imageEnergyGauge.fillAmount = energyCurrent;
        Vector2 gaugeDisplay = sizeEnergyGaugeHeight;
        gaugeDisplay.y *= energyCurrent;
        imageEnergyGauge.sizeDelta = gaugeDisplay;
        invincibilityPeriodCurrent -= Time.deltaTime;
        pauseCooldown -= Time.unscaledDeltaTime;
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
                            animatorPlayer.Play("PlayerBanged");
                            audioSourceSound.PlayOneShot(clipDrink);
                        }
                        break;
                    // Small obstacle
                    case GameInteractable.InteractableType.ObstacleSmall:
                        if (dist < interactableObstacleSmallColRadius)
                        {
                            if (!y.isInteracted && invincibilityPeriodCurrent <= 0f)
                            {
                                y.isInteracted = true;
                                stageMoveSpeedCurrent *= 0.5f;
                                energyCurrent -= interactableObstacleSmallEnergyDecrease;
                                invincibilityPeriodCurrent = invincibilityPeriodOnHitObstacleSmall;
                                animatorPlayer.Play("PlayerBanged");
                                audioSourceSound.PlayOneShot(clipHit);
                            }
                        }
                        break;
                    // Large obstacle
                    case GameInteractable.InteractableType.ObstacleLarge:
                        if (dist < interactableObstacleLargeColRadius)
                        {
                            if (!y.isInteracted && invincibilityPeriodCurrent <= 0f)
                            {
                                y.isInteracted = true;
                                stageMoveSpeedCurrent = 0f;
                                energyCurrent -= interactableObstacleLargeEnergyDecrease;
                                invincibilityPeriodCurrent = invincibilityPeriodOnHitObstacleLarge;
                                animatorPlayer.Play("PlayerBanged");
                                audioSourceSound.PlayOneShot(clipHit);
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

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title_Screen");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("game");
    }
}
