using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInteractable : MonoBehaviour
{
    public enum InteractableType
    {
        EnergyRestore,
        ObstacleSmall
    }

    public InteractableType mInteractableType = InteractableType.EnergyRestore;
    public bool isInteracted = false;
}
