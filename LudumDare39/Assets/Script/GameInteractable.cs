using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInteractable : MonoBehaviour
{
    public enum InteractableType
    {
        EnergyRestore,
        ObstacleSmall,
        ObstacleLarge
    }

    public InteractableType mInteractableType = InteractableType.EnergyRestore;
    public Vector3 movePoint = Vector3.zero;
    public bool isRotatingRandomly = false;
    private float floatRotationRate = 0f;
    public bool isInteracted = false;

    void Start()
    {
        if (isRotatingRandomly)
        {
            floatRotationRate = Random.Range(-180f, 180f);
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }
    }
    void Update()
    {
        if (isRotatingRandomly)
        {
            transform.Rotate(new Vector3(0f, 0f, floatRotationRate * Time.deltaTime));
        }
    }
}
