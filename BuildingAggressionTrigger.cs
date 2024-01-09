/*****************************************************************************
// File Name :         BuildingAggressionTrigger.cs
// Author :            Nick Grinstead
// Creation Date :     Nov 18th, 2023
//
// Brief Description :  This script is used by triggers to determine if the player
                        is near a building or not. It then makes the AggressionManager
                        toggle its area modifier accordingly.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAggressionTrigger : MonoBehaviour
{
    AggressionManager aggressionManager;

    private void Awake()
    {
        aggressionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<AggressionManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            aggressionManager.ToggleAreaModifier(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            aggressionManager.ToggleAreaModifier(true);
    }
}
