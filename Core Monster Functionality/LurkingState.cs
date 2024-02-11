/*****************************************************************************
// File Name :         LurkingState.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 30th, 2023
//
// Brief Description :  This MonsterState handles the behavior for moving from
                        hiding spot to hiding spot in order to approach the player.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LurkingState : MonsterState, Observer
{
    MonsterAI context;
    Transform playerTrans;

    NavMeshAgent monsterAgent;
    Vector2 repositionTimingMax;
    Vector2 repositionTimingMin;
    Vector2 currentRepositionTiming;
    float repositionTime;
    float timeWaited;
    bool isInHiding = false;
    Vector3 chosenSpot;

    public LurkingState(MonsterAI newContext, Transform playerTransform, 
        Vector2 maxTimeRange, Vector2 minTimeRange, ref NavMeshAgent navAgent)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Subject>().RegisterObserver(this);
        context = newContext;
        playerTrans = playerTransform;
        currentRepositionTiming = repositionTimingMax = maxTimeRange;
        repositionTimingMin = minTimeRange;
        timeWaited = repositionTime = Random.Range(currentRepositionTiming.x, currentRepositionTiming.y);
        monsterAgent = navAgent;
    }

    /// <summary>
    /// Gets modifier from AggressionManager
    /// </summary>
    /// <param name="modifier">Change to repositionTiming</param>
    public void ReceiveSubjectInfo(float modifier)
    {
        currentRepositionTiming.x = repositionTimingMax.x - 
            ( modifier * (repositionTimingMax.x - repositionTimingMin.x) );
        currentRepositionTiming.y = repositionTimingMax.y - 
            ( modifier * (repositionTimingMax.y - repositionTimingMin.y) );
    }

    /// <summary>
    /// Called to select NavAgent destination
    /// </summary>
    public void UpdateDestination()
    {
        chosenSpot = MonsterState.destination = PickHidingSpot();
    }

    /// <summary>
    /// Called to check if the destination needs to update
    /// </summary>
    /// <returns>True if new destination is needed, false if not</returns>
    public bool CheckForUpdate()
    {
        // Time to reposition
        if (timeWaited >= repositionTime)
        {
            timeWaited = 0;
            repositionTime = Random.Range(currentRepositionTiming.x, currentRepositionTiming.y);
            context.monsterAnimator.Play("Walk");
            isInHiding = false;
            return true;
        }
        // Should remain in place
        else if (isInHiding)
        {
            timeWaited += context.coroutineWaitTime;
            return false;
        }

        if (Mathf.Abs(Vector3.Distance(context.transform.position, chosenSpot)) <= 3.5f)
        {
            context.monsterAnimator.Play("Idle");
            isInHiding = true;
        }

        return false;
    }

    /// <summary>
    /// Called to check if the monster should leave this state
    /// </summary>
    public void CheckForStateChange()
    {
        // Checks if player is close enough to attack
        if (Physics.CheckSphere(context.transform.position, 32, LayerMask.GetMask("Player")))
        {
            ResetState();
            context.FadeSound(true);
            context.currentState = context.attacking;
        }
        else if (Mathf.Abs(Vector3.Distance(playerTrans.position, context.transform.position)) >= 80)
        {
            ResetState();
            context.currentState = context.attacking;
        }
    }

    /// <summary>
    /// Called to reset values used in this state
    /// </summary>
    public void ResetState()
    {
        context.monsterAnimator.Play("Walk");
        timeWaited = repositionTime;
    }

    /// <summary>
    /// Chooses a hiding spot at random that isn't the hiding spot that's closest
    /// to the player
    /// </summary>
    private Vector3 PickHidingSpot()
    {
        int closestObjectIndex = 0;
        float minDistance = Mathf.Infinity;
        float currentDistance;

        Collider[] overlapResult = Physics.OverlapSphere(context.transform.position, 40, LayerMask.GetMask("HidingSpot"));
        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < overlapResult.Length; ++i)
        {
            currentDistance = Mathf.Abs(Vector3.Distance(overlapResult[i].transform.position, playerTrans.position));

            if (currentDistance < minDistance &&
                monsterAgent.CalculatePath(overlapResult[i].transform.position, path)
                && path.status == NavMeshPathStatus.PathComplete)

            {
                minDistance = currentDistance;
                closestObjectIndex = i;
            }
        }

        // Picking a corner of the hiding spot
        if (overlapResult.Length != 0)
        {
            Collider tempCollider = overlapResult[closestObjectIndex].GetComponent<Collider>();
            Vector3[] vertices = {
            new Vector3(tempCollider.bounds.min.x, tempCollider.bounds.min.y, tempCollider.bounds.min.z),
            new Vector3(tempCollider.bounds.max.x, tempCollider.bounds.min.y, tempCollider.bounds.min.z),
            new Vector3(tempCollider.bounds.max.x, tempCollider.bounds.min.y, tempCollider.bounds.max.z),
            new Vector3(tempCollider.bounds.min.x, tempCollider.bounds.min.y, tempCollider.bounds.max.z),
        };

            int vertexIndex = 0;
            float shortestDistance = Mathf.Infinity;

            for (int i = 0; i < vertices.Length; ++i)
            {
                float distance = Mathf.Abs(Vector3.Distance(playerTrans.position, vertices[i]));

                if (distance < shortestDistance)
                {
                    vertexIndex = i;
                    shortestDistance = distance;
                }
            }

            // Monster goes for player instead if hiding spot would lead away from player
            if (shortestDistance > Mathf.Abs(Vector3.Distance(playerTrans.position, context.transform.position)))
            {
                return playerTrans.position;
            }

            int chosenIndex = vertexIndex;

            do
            {
                chosenIndex = Random.Range(0, vertices.Length);
            } while (vertexIndex == chosenIndex);

            return vertices[chosenIndex];
        }
        else
        {
            return playerTrans.position;
        }
    }
}
