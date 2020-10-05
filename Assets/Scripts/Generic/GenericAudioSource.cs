using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAudioSource : MonoBehaviour
{
    
    protected AudioSource audioSource;

    [SerializeField]
    [Tooltip("Audio handler containing resources for audiosource to play. Can be in parent or defined here")]
    protected GenericAudioHandler audioHandler = null;

    // Internal list updated each time it plays audio
    protected float pitchMin, pitchMax, volumeMin, volumeMax;

    // Find audio handler on start
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Try to get audio handler from parent if reference not set in editor
        if (audioHandler == null)
            audioHandler = gameObject.GetComponentInParent<GenericAudioHandler>();

        if (audioHandler == null)
            throw new System.Exception("No audio handler found", null);

        UpdateVariablesFromHandler();
    }

    // Update variables from audioHandler
    virtual protected void UpdateVariablesFromHandler()
    {
        pitchMax = audioHandler.pitchMax;
        pitchMin = audioHandler.pitchMin;
        volumeMax = audioHandler.volumeMax;
        volumeMin = audioHandler.volumeMin;
    }

    virtual public void PlaySound()
    {
        // Stop if still playing sound
        if (audioSource.isPlaying)
            return;

        // Add variations to sound
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = Random.Range(volumeMin, volumeMax);

        // Play sound
        audioSource.clip = audioHandler.GetAudioClip();
        audioSource.Play();
    }



}