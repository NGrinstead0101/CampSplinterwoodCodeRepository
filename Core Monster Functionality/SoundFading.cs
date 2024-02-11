/*****************************************************************************
// File Name :         BuildingAggressionTrigger.cs
// Author :            Nick Grinstead
// Creation Date :     Nov 27th, 2023
//
// Brief Description :  This script fades out certain background SFX when the 
                        monster is in its attacking state and fades them back in
                        once the monster leaves that state.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFading : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    float initialVolume;
    [SerializeField] float fadeTime;

    private void Awake()
    {
        initialVolume = soundSource.volume;
    }

    /// <summary>
    /// Called to start the sound fade in or out
    /// </summary>
    /// <param name="shouldFadeOut">If true fade out, if false fade in</param>
    public void StartFading(bool shouldFadeOut)
    {
        if (shouldFadeOut)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Coroutine that fades out background SFX until the volume hits 0
    /// </summary>
    IEnumerator FadeOut()
    {
        while (soundSource.volume > 0)
        {
            soundSource.volume -= initialVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        soundSource.Stop();
        soundSource.volume = 0f;
    }

    /// <summary>
    /// Coroutine that fades in background SFX until the volume reaches its max
    /// </summary>
    IEnumerator FadeIn()
    {
        soundSource.Play();

        while (soundSource.volume < 1f)
        {
            soundSource.volume += initialVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        soundSource.volume = 1f;
    }
}
