/*****************************************************************************
// File Name :         LightDetection.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 7th, 2023
//
// Brief Description :  This class handles firing raycasts from the player's
                        flashlight in order to determine if the monster has
                        been hit by the light.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightDetection : MonoBehaviour, Observer
{
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;
    float currentAngle;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    float currentDistance;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] int frameInterval;

    MonsterAI monsterAI;
    Transform monsterTrans;
    bool isLightOn = false;

    /// <summary>
    /// Initializing variables
    /// </summary>
    private void Awake()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Monster");
        monsterAI = temp.GetComponent<MonsterAI>();
        monsterTrans = temp.GetComponent<Transform>();
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Subject>().RegisterObserver(this);
        currentAngle = maxAngle;
        currentDistance = maxDistance;

        if (frameInterval <= 0)
            frameInterval = 5;
    }

    /// <summary>
    /// Activates or deactivates raycasting and NavMeshObstacle
    /// </summary>
    public void ToggleLightObstacle(bool isActive)
    {
        isLightOn = isActive;
    }

    /// <summary>
    /// Gets modifier from the AggressionManager
    /// </summary>
    /// <param name="modifier">Change distance and angle</param>
    public void ReceiveSubjectInfo(float modifier)
    {
        currentAngle = maxAngle - ( modifier * (maxAngle - minAngle) );
        currentDistance = maxDistance - ( modifier * (maxDistance - minDistance) );
    }

    /// <summary>
    /// Fires a raycast and determines if it hit within the light cone
    /// </summary>
    void Update()
    {
        if (isLightOn && Time.frameCount % frameInterval == 0)
        {
            // Checks if the raycast hit its target
            if (Physics.Raycast(transform.position, monsterTrans.position - transform.position, currentDistance, targetLayer, QueryTriggerInteraction.Ignore))
            {
                // Calculates if angle of raycast falls within the angle of the flashlight
                if (Mathf.Abs(Vector3.SignedAngle(monsterTrans.position - transform.position, transform.forward, Vector3.up)) <= currentAngle)
                {
                    monsterAI.FleeLight(transform.position);
                }
            }
        }
    }
}
