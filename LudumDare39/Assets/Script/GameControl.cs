using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject objectPlayer;
    private CircleCollider2D colliderPlayer;
    public float playerMoveSpeed = 5f;
    public Vector2 playerBounds = Vector2.one * 6f;

    public float energyCurrent = 1f;
    public float energyDecreaseRate = 0.01f;
    public float energyRecoverOnItemPickup = 0.4f;

    public float stageMoveSpeedCurrent = 0f;
    public float stageMoveSpeedAcceleration = 2.5f;
    public float stageMoveSpeedMaximum = 6f;

    void Start()
    {
        colliderPlayer = objectPlayer.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (energyCurrent > 0f)
        {
            stageMoveSpeedCurrent = Mathf.Clamp(stageMoveSpeedCurrent + (stageMoveSpeedAcceleration * Time.deltaTime), 0f, stageMoveSpeedMaximum);
        }
        else
        {
            stageMoveSpeedCurrent = Mathf.Clamp(stageMoveSpeedCurrent - (stageMoveSpeedAcceleration * Time.deltaTime), 0f, stageMoveSpeedMaximum);
        }

        if (energyCurrent > Mathf.Epsilon || stageMoveSpeedCurrent > Mathf.Epsilon)
        {
            float moveX = Input.GetAxisRaw("Horizontal") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum;
            float moveY = Input.GetAxisRaw("Vertical") * playerMoveSpeed * stageMoveSpeedCurrent / stageMoveSpeedMaximum;

            Vector3 playerPos = objectPlayer.transform.localPosition;
            playerPos.x = Mathf.Clamp(playerPos.x + moveX, -playerBounds.x, playerBounds.x);
            playerPos.y = Mathf.Clamp(playerPos.y + moveY, -playerBounds.y, playerBounds.y);
            objectPlayer.transform.localPosition = playerPos;
        }
    }
}
