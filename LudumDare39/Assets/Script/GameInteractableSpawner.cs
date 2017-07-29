using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInteractableSpawner : MonoBehaviour
{
    public List<GameInteractable> listInteractable = new List<GameInteractable>();
    public Rect rectInteractableSpawnPos = new Rect(0f, 9f, 10f, 4f);

    public GameInteractable interactablePrefab;
    private float interactableSpawnDelayCurrent = 0f;
    private float interactableSpawnDelayTimer = 0f;
    public float interactableSpawnDelayInitial = 30f;
    public float interactableSpawnDelayAdjustment = 1f;
    public float interactableSpawnDelayMinimum = 10f;
    public float interactableSpawnDelayMaximum = 50f;

    public void SpawnInteractable(GameInteractable prefab)
    {
        GameInteractable obj = Instantiate(prefab);
        Vector3 objSpawnPos = new Vector3(
            rectInteractableSpawnPos.position.x + Random.Range(-rectInteractableSpawnPos.width / 2f, rectInteractableSpawnPos.width / 2f),
            rectInteractableSpawnPos.position.y + Random.Range(-rectInteractableSpawnPos.height / 2f, rectInteractableSpawnPos.height / 2f)
            );
        obj.transform.position = objSpawnPos;
        listInteractable.Add(obj);
    }

    void Start ()
    {
        interactableSpawnDelayTimer = interactableSpawnDelayCurrent = interactableSpawnDelayInitial;
    }
	
	void Update ()
    {
        // Do nothing if game has ended.
        if (GameControl.control.isGameOver)
        {
            return;
        }

        // Spawn timer
        interactableSpawnDelayTimer -= Time.deltaTime * GameControl.control.stageMoveSpeedCurrent / GameControl.control.stageMoveSpeedMaximum;

        // Instantiate on timer 0
        if (interactableSpawnDelayTimer < 0f)
        {
            SpawnInteractable(interactablePrefab);
            interactableSpawnDelayCurrent = Mathf.Clamp(interactableSpawnDelayCurrent + interactableSpawnDelayAdjustment, interactableSpawnDelayMinimum, interactableSpawnDelayMaximum);
            interactableSpawnDelayTimer = interactableSpawnDelayCurrent;
        }

        // Move existing objects
        for (int i = 0; i < listInteractable.Count; i++)
        {
            // Ignore empty objects
            if (listInteractable[i] == null)
            {
                continue;
            }

            listInteractable[i].transform.position +=
                (Vector3.down * GameControl.control.stageMoveSpeedCurrent + listInteractable[i].movePoint) * Time.deltaTime;
        }

        // Destroying objects far below play field or is already destroyed
        for (int i = 0; i < listInteractable.Count; i++)
        {
            if (listInteractable[i] == null)
            {
                listInteractable.RemoveAt(i);
                i--;
            }
            else if (listInteractable[i].transform.position.y < -rectInteractableSpawnPos.position.y - rectInteractableSpawnPos.height)
            {
                Destroy(listInteractable[i].gameObject);
                listInteractable.RemoveAt(i);
                i--;
            }
        }
    }
}
