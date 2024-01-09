/*****************************************************************************
// File Name :         FleeingState.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 30th, 2023
//
// Brief Description :  This MonsterState handles the behavior for running from
                        the light to the furthest reachable hiding spot.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeingState : MonsterState, Observer
{
    MonsterAI context;
    bool spotChosen = false;
    Vector3 chosenSpot;

    Transform playerTrans;
    GameObject[] hidingSpots;
    Vector2 initialHidingDistances;
    Vector2 finalHidingDistances;
    float currentMinDistance;
    float currentMaxDistance;

    NavMeshAgent monsterAgent;

    public FleeingState(MonsterAI newContext, ref Transform playerTransform,
        ref GameObject[] hidingSpots, Vector2 escapeDistance, Vector2 maxDistance, ref NavMeshAgent navAgent)
    {
        context = newContext;
        playerTrans = playerTransform;
        this.hidingSpots = hidingSpots;
        initialHidingDistances = escapeDistance;
        finalHidingDistances = maxDistance;
        currentMinDistance = initialHidingDistances.x;
        currentMaxDistance = initialHidingDistances.y;
        monsterAgent = navAgent;
    }

    public void ReceiveSubjectInfo(float modifier)
    {
        currentMinDistance = initialHidingDistances.x - (modifier * (initialHidingDistances.x - finalHidingDistances.x));
        currentMaxDistance = initialHidingDistances.y - (modifier * (initialHidingDistances.y - finalHidingDistances.y));
    }

    /// <summary>
    /// Picks a hiding spot as the destination
    /// </summary>
    public void UpdateDestination()
    {
        chosenSpot = MonsterState.destination = PickHidingSpot();
    }

    /// <summary>
    /// Returns true if a spot hasn't been chosen, and false otherwise
    /// </summary>
    /// <returns>Whether a new destination is needed</returns>
    public bool CheckForUpdate()
    {
        if (!spotChosen)
        {
            spotChosen = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the monster has reached its hiding spot
    /// </summary>
    public void CheckForStateChange()
    {
        if (Mathf.Abs(Vector3.Distance(context.transform.position, chosenSpot)) <= 5)
        {
            ResetState();
            context.meshRenderer.enabled = false;
            context.currentState = context.hiding;
        }
    }

    /// <summary>
    /// Resets values used in this state
    /// </summary>
    public void ResetState()
    {
        spotChosen = false;
        context.SwitchToRunning(false);
    }

    /// <summary>
    /// Picks a hiding spot that's furthest from the player and then picks
    /// a corner of it
    /// </summary>
    /// <returns>Vector3 representing the target location</returns>
    private Vector3 PickHidingSpot()
    {
        int chosenHidingSpot = 0;
        bool spotFound = false;

        NavMeshPath path = new NavMeshPath();

        int checkCount = 0;
        int indexToCheck = Random.Range(0, hidingSpots.Length);
        float hidingSpotDistance;

        for (; checkCount < hidingSpots.Length;)
        {
            hidingSpotDistance = Vector3.Distance(hidingSpots[indexToCheck].transform.localPosition, playerTrans.position);

            if (hidingSpotDistance >= currentMinDistance && hidingSpotDistance <= currentMaxDistance &&
                monsterAgent.CalculatePath(hidingSpots[indexToCheck].transform.position, path) && 
                path.status == NavMeshPathStatus.PathComplete)
            {
                chosenHidingSpot = indexToCheck;
                spotFound = true;
                break;
            }

            checkCount++;
            indexToCheck = (indexToCheck + 1) % hidingSpots.Length;
        }

        if (spotFound == false)
        {
            checkCount = 0;
            indexToCheck = Random.Range(0, hidingSpots.Length);

            for (; checkCount < hidingSpots.Length;)
            {
                hidingSpotDistance = Vector3.Distance(hidingSpots[indexToCheck].transform.localPosition, playerTrans.position);

                if (hidingSpotDistance >= currentMinDistance && hidingSpotDistance <= currentMaxDistance * 2 &&
                    monsterAgent.CalculatePath(hidingSpots[indexToCheck].transform.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    chosenHidingSpot = indexToCheck;
                    spotFound = true;
                    break;
                }

                checkCount++;
                indexToCheck = (indexToCheck + 1) % hidingSpots.Length;
            }
        }

        if (spotFound == false)
        {
            checkCount = 0;
            indexToCheck = Random.Range(0, hidingSpots.Length);

            for (; checkCount < hidingSpots.Length;)
            {
                hidingSpotDistance = Vector3.Distance(hidingSpots[indexToCheck].transform.localPosition, playerTrans.position);

                if (hidingSpotDistance >= currentMinDistance && 
                    monsterAgent.CalculatePath(hidingSpots[indexToCheck].transform.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    chosenHidingSpot = indexToCheck;
                    spotFound = true;
                    break;
                }

                checkCount++;
                indexToCheck = (indexToCheck + 1) % hidingSpots.Length;
            }
        }

        // Picking a corner of the hiding spot

        Collider tempCollider = hidingSpots[chosenHidingSpot].GetComponent<Collider>();
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

        int chosenIndex = vertexIndex;

        do
        {
            chosenIndex = Random.Range(0, vertices.Length);
        } while (vertexIndex == chosenIndex);

        return vertices[chosenIndex];
    }
}

