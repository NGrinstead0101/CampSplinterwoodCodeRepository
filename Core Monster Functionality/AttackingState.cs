/*****************************************************************************
// File Name :         AttackingState.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 30th, 2023
//
// Brief Description :  This MonsterState handles the behavior for moving directly
                        toward the player when they're in range.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : MonsterState
{
    MonsterAI context;
    Transform playerTrans;

    public AttackingState(MonsterAI newContext, ref Transform playerTransform)
    {
        context = newContext;
        playerTrans = playerTransform;
    }

    /// <summary>
    /// Sets destination to the player's position
    /// </summary>
    public void UpdateDestination()
    {
        MonsterState.destination = playerTrans.position;
    }

    /// <summary>
    /// This state always updates the destination
    /// </summary>
    /// <returns>true</returns>
    public bool CheckForUpdate()
    {
        return true;
    }

    /// <summary>
    /// Checks if player has moved away to determine if the state should change
    /// </summary>
    public void CheckForStateChange()
    {
        if (!Physics.CheckSphere(context.transform.position, 30, LayerMask.GetMask("Player"))
            && Mathf.Abs(Vector3.Distance(playerTrans.position, context.transform.position)) < 70)
        {
            ResetState();
            context.FadeSound(false);
            context.currentState = context.lurking;
        }
    }

    /// <summary>
    /// This state has no values to reset
    /// </summary>
    public void ResetState()
    {
    }
}
