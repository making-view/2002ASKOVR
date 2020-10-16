using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericAudioSource : MonoBehaviour
{

    protected List<AudioSource> audioSources;

    [SerializeField]
    [Tooltip("Audio handler containing resources for audiosource to play. Can be in parent or defined here")]
    protected GenericAudioHandler audioHandler = null;
    [SerializeField]
    [Range(1, 5)]
    protected int maxConcurrentSounds = 2;

    // Internal list updated each time it plays audio
    protected float pitchMin, pitchMax, volumeMin, volumeMax;

    // Find audio handler on start
    void Start()
    {
        audioSources = new List<AudioSource>();

        for (int i = 0; i < maxConcurrentSounds; ++i)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
            audioSources[i].playOnAwake = false;
            audioSources[i].spatialBlend = 0.5f;
            audioSources[i].minDistance = 0.1f;
            audioSources[i].maxDistance = 50f;
        }

        // Try to get audio handler from parent if reference not set in editor
        if (audioHandler == null)
            audioHandler = gameObject.GetComponentInParent<GenericAudioHandler>();

        if (audioHandler == null)
            throw new System.Exception("No audio handler found", null);

        UpdateVariablesFromHandler();
    }

    // Update variables from audioHandler
    virtual public void UpdateVariablesFromHandler()
    {
        pitchMax = audioHandler.pitchMax;
        pitchMin = audioHandler.pitchMin;
        volumeMax = audioHandler.volumeMax;
        volumeMin = audioHandler.volumeMin;
    }

    virtual public void PlaySound()
    {
        var availableSource = audioSources.FirstOrDefault(a => !a.isPlaying);

        if (availableSource)
        {
            // Add variations to sound
            availableSource.pitch = Random.Range(pitchMin, pitchMax);
            availableSource.volume = Random.Range(volumeMin, volumeMax);

            // Play sound
            availableSource.clip = audioHandler.GetAudioClip();
            availableSource.Play();
        }
    }
}