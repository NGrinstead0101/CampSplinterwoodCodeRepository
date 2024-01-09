/*****************************************************************************
// File Name :         HidingState.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 30th, 2023
//
// Brief Description :  This MonsterState handles the behavior for remaining
                        invisible at a hiding spot after running and deciding
                        when to leave that spot.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingState : MonsterState, Observer
{
    MonsterAI context;
    float maxHidingTime;
    float minHidingTime;
    float currentHidingTime;
    float timeWaited;

    public HidingState(MonsterAI newContext, float maxTime, float minTime)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Subject>().RegisterObserver(this);
        context = newContext;
        currentHidingTime = maxHidingTime = maxTime;
        minHidingTime = minTime;
        timeWaited = 0;
    }

    /// <summary>
    /// Gets modifier from AggressionManager
    /// </summary>
    /// <param name="modifier">Change to hiding time</param>
    public void ReceiveSubjectInfo(float modifier)
    {
        currentHidingTime = maxHidingTime - ( modifier * (maxHidingTime - minHidingTime) );
    }

    /// <summary>
    /// Destination doesn't need to update in this state
    /// </summary>
    public void UpdateDestination()
    {
    }

    /// <summary>
    /// Destination will never update in this state
    /// </summary>
    /// <returns>false</returns>
    public bool CheckForUpdate()
    {
        return false;
    }

    /// <summary>
    /// Checks time and player distance to determine if monster should change states
    /// </summary>
    public void CheckForStateChange()
    {
        timeWaited += context.coroutineWaitTime;

        if (timeWaited >= currentHidingTime && 
            !Physics.CheckSphere(context.transform.position, 40, LayerMask.GetMask("Player")))
        {
            ResetState();
            context.currentState = context.lurking;
        }
    }

    /// <summary>
    /// Resets values used in this state
    /// </summary>
    public void ResetState()
    {
        timeWaited = 0;
        context.meshRenderer.enabled = true;
    }
}
