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
    public bool isInteracted = false;
}
