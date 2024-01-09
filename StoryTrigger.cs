/*****************************************************************************
// File Name :         StoryTrigger.cs
// Author :            Nick Grinstead
// Creation Date :     Oct 5th, 2023
//
// Brief Description :  Story trigger for when the player enters a new location.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    bool hasTriggered = false;
    AggressionManager aggressionManager;
    GameController gc;

    [SerializeField] string targetItem;
    [SerializeField] string secondTargetItem;

    /// <summary>
    /// Getting reference to AggressionManager
    /// </summary>
    private void Awake()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("GameController");
        aggressionManager = temp.GetComponent<AggressionManager>();
        gc = temp.GetComponent<GameController>();

        if (PlayerPrefs.HasKey(gameObject.name))
        {
            hasTriggered = PlayerPrefs.GetInt(gameObject.name) == 1;
        }
    }

    /// <summary>
    /// Checks if the player has one or both required items for this trigger
    /// </summary>
    /// <returns></returns>
    private bool CheckForItem()
    {
        // Checks for bathroom note
        if (targetItem.Equals("Note") && gc.bathroomNote.activeSelf)
        {
            return true;
        }
        
        // Checks if you have one or both required items
        if (gc.Items.Contains(targetItem))
        {
            if (secondTargetItem == null || gc.Items.Contains(secondTargetItem))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The first time the player enters the trigger, increase the monster's aggression
    /// </summary>
    /// <param name="other">Collider involved in collision</param>
    private void OnTriggerStay(Collider other)
    {
        if (hasTriggered && other.CompareTag("Player"))
            return;

        if (other.CompareTag("Player") && CheckForItem())
        {
            hasTriggered = true;
            PlayerPrefs.SetInt(gameObject.name, 1);
            aggressionManager.StoryTriggerHit();
        }
    }
}
