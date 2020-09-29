using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    const int CLIP = 0;
    const int LOOP = 1;

    [SerializeField] AudioClip start;
    [SerializeField] AudioClip loop;
    [SerializeField] AudioClip end;

    [SerializeField] AudioSource[] audioSources;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        StartCoroutine(PlaySeamless());
    }

    private IEnumerator PlaySeamless()
    {   
        audioSources[CLIP].clip = start;
        audioSources[CLIP].Play();
        audioSources[LOOP].clip = loop;
        audioSources[LOOP].loop = true;
        audioSources[LOOP].PlayDelayed(start.length);

        while (!audioSources[LOOP].isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(5);

        audioSources[LOOP].loop = false;
        audioSources[CLIP].clip = end;
        audioSources[CLIP].PlayDelayed(loop.length - audioSources[LOOP].time);
    }
}
