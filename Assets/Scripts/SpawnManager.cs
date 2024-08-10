using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GroundSpawner groundSpawner;

    public void SpawnTriggerEntered(float trainSpeed, string colliderName)
    {
        if (trainSpeed > 0 && colliderName.Contains("Front"))
        {
            groundSpawner.MoveGroundInFront();
        }

        if (trainSpeed < 0 && colliderName.Contains("Back"))
        {
            groundSpawner.MoveGroundBehind();
        }
    }
}
