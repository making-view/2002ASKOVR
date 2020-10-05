using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct containing lists of audioclips
[System.Serializable]
public struct ClipList
{
    public string name;
    public List<AudioClip> clip;
}

public class GenericAudioHandler : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [Tooltip("Minimum volume in random variation")]
    public float volumeMin = 0.8f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Maximum volume in random variation")]
    public float volumeMax = 1.0f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Minimum pitch in random variation")]
    public float pitchMin = 0.8f;

    [Range(0.0f, 2.0f)]
    [Tooltip("Maximum pitch in random variation")]
    public float pitchMax = 1.2f;

    [SerializeField]
    protected string startingClips = "";

    // Named lists of audioclips
    [SerializeField]
    public ClipList[] audioClips = null;
    protected List<AudioClip> activeClips = null;
    private AudioClip lastPlayed = null;

    // Make sure min variables aren't higher than max variables
    // set starting clips to the one named in editor
    private void Start()
    {
        if (pitchMin > pitchMax)
            pitchMin = pitchMax;

        if (volumeMin > volumeMax)
            volumeMin = volumeMax;

        if (!startingClips.Equals(""))
            UpdateActiveClips(startingClips);
    }

    // Get previously played audioclip
    public AudioClip GetAudioClip()
    {
        List<AudioClip> clipsToPlay = new List<AudioClip>(activeClips);

        // Don't play same as last time
        if (lastPlayed != null && clipsToPlay.Count > 1)
            clipsToPlay.Remove(lastPlayed);

        // Find new clip to play
        AudioClip clip = clipsToPlay[Random.Range(0, clipsToPlay.Count - 1)];

        lastPlayed = clip;

        return clip;
    }

    // Get list with name defined in editor
    protected void UpdateActiveClips(string nameOfList)
    {
        foreach(ClipList c in audioClips)
        {
            if (c.name.ToUpper().Equals(nameOfList.ToUpper()))
            {
                activeClips = c.clip;
                return;
            }
        }
    }
}