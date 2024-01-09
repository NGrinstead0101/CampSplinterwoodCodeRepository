/*****************************************************************************
// File Name :         MonsterAI.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 5th, 2023
//
// Brief Description :  Main script for handling how the monster moves and acts.
                        Shifts between three distinct movement states to determine
                        how the monsters destination is calculated.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour, Observer
{
    [SerializeField] NavMeshAgent navAgent;
    Transform playerTrans;
    public SkinnedMeshRenderer meshRenderer;
    public Animator monsterAnimator;

    GameObject[] hidingSpots;
    [SerializeField] Vector2 startingRepositionTiming;
    [SerializeField] Vector2 minRepositionTiming;
    [SerializeField] Light flashlight;

    float normalMoveSpeed;
    [SerializeField] float startingMoveSpeed;
    [SerializeField] float finalMoveSpeed;
    [SerializeField] float fleeingMoveSpeed;

    public bool isActive = true;
    // Determines how often coroutine runs
    public float coroutineWaitTime;
    [SerializeField] float maxHidingTime;
    [SerializeField] float minHidingTime;

    public MonsterState currentState;
    public MonsterState fleeing, attacking, lurking, hiding;

    [SerializeField] Vector2 initialEscapeDistances;
    [SerializeField] Vector2 finalEscapeDistances;
    [SerializeField] LightDetection lightDetectionScript;
    [SerializeField] SoundFading soundFading;

    /// <summary>
    /// Initializing variables and starting movement
    /// </summary>
    private void Awake()
    {
        hidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");

        GameObject temp = GameObject.FindGameObjectWithTag("GameController");
        temp.GetComponent<Subject>().RegisterObserver(this);
        soundFading = temp.GetComponent<SoundFading>();

        playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        monsterAnimator = GetComponent<Animator>();

        normalMoveSpeed = startingMoveSpeed;

        fleeing = new FleeingState(this, ref playerTrans, ref hidingSpots, initialEscapeDistances, finalEscapeDistances, ref navAgent);
        attacking = new AttackingState(this, ref playerTrans);
        lurking = new LurkingState(this, playerTrans, startingRepositionTiming, minRepositionTiming, ref navAgent);
        hiding = new HidingState(this, maxHidingTime, minHidingTime);
        currentState = lurking;

        SwitchToRunning(false);

        StartCoroutine(Move());
    }

    /// <summary>
    /// Handles updating the monster's movement target based on its current state
    /// </summary>
    /// <returns>Waits a number of seconds based on coroutineWaitTime</returns>
    IEnumerator Move()
    {
        while (true)
        {
            if (isActive)
            {
                currentState.CheckForStateChange();

                if (currentState.CheckForUpdate())
                {
                    currentState.UpdateDestination();
                    if (navAgent.enabled)
                    {
                        navAgent.SetDestination(MonsterState.destination);
                    }
                }
            }

            yield return new WaitForSeconds(coroutineWaitTime);
        }
    }

    public void ToggleModel(bool isActive)
    {
        meshRenderer.enabled = isActive;
    }

    public void FadeSound(bool shouldFadeOut)
    {
        soundFading.StartFading(shouldFadeOut);
    }

    /// <summary>
    /// Receives aggression modifier and then modifies the monster's movement speed
    /// </summary>
    /// <param name="modifier">The aggression modifier</param>
    public void ReceiveSubjectInfo(float modifier)
    {
        normalMoveSpeed = startingMoveSpeed + (modifier * (finalMoveSpeed - startingMoveSpeed));

        if (navAgent.speed != fleeingMoveSpeed)
            SwitchToRunning(false);
    }

    /// <summary>
    /// Updates the monster's speed
    /// </summary>
    /// <param name="isRunning">Whether the monster is running</param>
    public void SwitchToRunning(bool isRunning)
    {
        if (isRunning)
            navAgent.speed = fleeingMoveSpeed;
        else
            navAgent.speed = normalMoveSpeed;
    }

    /// <summary>
    /// Called by LightDetection class when raycast hits
    /// Will calculate where the monster will run away to
    /// </summary>
    /// <param name="lightPosition"></param>
    public void FleeLight(Vector3 lightPosition)
    {
        // Only runs if the monster isn't already fleeing
        if (currentState != fleeing && currentState != hiding)
        {
            currentState.ResetState();
            FadeSound(false);
            currentState = fleeing;
            navAgent.speed = fleeingMoveSpeed;

            if (currentState.CheckForUpdate())
            {
                currentState.UpdateDestination();
                navAgent.SetDestination(MonsterState.destination);
            }
        }
    }

    /// <summary>
    /// Goes to lose screen when monster catches player
    /// </summary>
    /// <param name="collision">Data related to a collision</param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && 
    //        currentState != fleeing && currentState != hiding)
    //    {
    //        //CampfireBehavior.staticInstance.Save(false);

    //        //SceneManager.LoadScene("LoseScreen");
    //    }
    //}
}
