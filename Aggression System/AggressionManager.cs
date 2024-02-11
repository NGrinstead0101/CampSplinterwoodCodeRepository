/*****************************************************************************
// File Name :         AggressionManager.cs
// Author :            Nick Grinstead
// Creation Date :     Oct 3rd, 2023
//
// Brief Description :  This script uses the number of story triggers that have
                        been reached along with fear to determine a modifier to
                        apply to certain variables in order to increase monster
                        aggression.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressionManager : MonoBehaviour, Subject
{
    float lightModifier = 0;
    float storyTriggersHit;
    [SerializeField] int totalStoryTriggers;
    [SerializeField] float areaModifier;
    [SerializeField] FlashLightBehavior flashlightBehavior;

    public float testingModifier;

    bool playerInOpen;

    List<Observer> observerList = new List<Observer>();

    private void Start()
    {
        if (PlayerPrefs.HasKey("StoryTriggersHit"))
        {
            storyTriggersHit = PlayerPrefs.GetFloat("StoryTriggersHit");
            UpdateObservers();
        }
        else
        {
            storyTriggersHit = 0;
        }

        totalStoryTriggers += (int) areaModifier;
    }

    private void Update()
    {
        if (Time.frameCount % 10 == 0 && flashlightBehavior.lightInHand())
            UpdateLightModifier(flashlightBehavior.getFlashLightIntensity());
    }

    /// <summary>
    /// Increments storyTriggersHit then updates all Observers
    /// </summary>
    public void StoryTriggerHit()
    {
        storyTriggersHit++;

        PlayerPrefs.SetFloat("StoryTriggersHit", storyTriggersHit);

        UpdateObservers();
    }

    public void ToggleAreaModifier(bool isInOpen)
    {
        playerInOpen = isInOpen;

        UpdateObservers();
    }

    /// <summary>
    /// Called by SanityManeagment to give a new fear value
    /// </summary>
    /// <param name="lightIntensity">The player's current fear</param>
    public void UpdateLightModifier(float lightIntensity)
    {
        float tempModifier = lightIntensity <= 0.3f ? 1f : lightIntensity <= 2.9f ? 0.6f : 0.0f;

        if (tempModifier != lightModifier)
        {
            lightModifier = tempModifier;
            UpdateObservers();
        }
    }

    /// <summary>
    /// Registers a new Observer to the list
    /// </summary>
    /// <param name="newObserver">The Observer to be registered</param>
    public void RegisterObserver(Observer newObserver)
    {
        observerList.Add(newObserver);

        UpdateObservers();
    }

    /// <summary>
    /// Removes an Observer from the list
    /// </summary>
    /// <param name="observerToRemove">Observer being removed</param>
    public void RemoveObserver(Observer observerToRemove)
    {
        observerList.Remove(observerToRemove);
    }

    /// <summary>
    /// Calls ReceiveSubjectInfo on all Observers in order to pass them updated information
    /// </summary>
    public void UpdateObservers()
    {
        foreach (Observer observer in observerList)
        {
            if (playerInOpen)
                observer.ReceiveSubjectInfo( (storyTriggersHit + lightModifier + areaModifier)/ totalStoryTriggers );
            else
                observer.ReceiveSubjectInfo((storyTriggersHit + lightModifier) / totalStoryTriggers);
        }

        if (playerInOpen)
            testingModifier = (storyTriggersHit + lightModifier + areaModifier) / totalStoryTriggers;
        else
            testingModifier = (storyTriggersHit + lightModifier) / totalStoryTriggers;
    }
}
