using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class JourneyHandler : MonoBehaviour
{
    [Serializable]
    public class JourneyEvent
    {
        [Serializable]
        public enum EventType
        {
            Action,
            Video
        }

        public string name = "event";
        public float duration = 1f;
        public EventType type;
        public Animation animation = null;
        public string animationName = "";
        public AudioClip narration = null;
        public ParticleSystem particles = null;
        public UnityEvent actions = null;

    }

    private BezierSolution.BezierSqauencer sequencer;
    private AudioSource narrator;
    [SerializeField] int debugStartAtEvent = 0;
    [SerializeField] List<JourneyEvent> journey = null;

    private int currentEvent = 0;
    private float nextEvent = 5.0f;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        narrator = GetComponent<AudioSource>();
        //BeginPlay();


        if (Application.isEditor)
            currentEvent = debugStartAtEvent;
    }


    public void BeginPlay(int eventnum)
    {
        StopCoroutine(PlayEvent());
        StopEvent();

        if (Application.isEditor && debugStartAtEvent != 0)
            currentEvent = debugStartAtEvent;
        else
            currentEvent = eventnum;

        if (journey.Count > currentEvent + 1)
            StartCoroutine(PlayEvent());
    }

    private void StopEvent()
    {
        if (journey[currentEvent].animation != null)
            journey[currentEvent].animation.Stop();

        //if (journey[currentEvent].moveThing != null)


        if (journey[currentEvent].particles != null)
            journey[currentEvent].particles.Stop();

        narrator.Stop();
    }

    private IEnumerator PlayEvent()
    {
        //Debug.Log("playing event: " + currentEvent);

        nextEvent = journey[currentEvent].duration;

        if (journey[currentEvent].animation != null)
        {
            var animname = journey[currentEvent].animationName;

            if (animname == "")
                journey[currentEvent].animation.Play();
            else
                journey[currentEvent].animation.Play(animname);
        }
            

        if (journey[currentEvent].particles != null)
            journey[currentEvent].particles.Play();


        if (journey[currentEvent].narration != null)
        {
            narrator.PlayOneShot(journey[currentEvent].narration);
            nextEvent += narrator.clip.length;
        }

        journey[currentEvent].actions.Invoke();

        yield return new WaitForSeconds(nextEvent);

        if (journey.Count > currentEvent + 1)
        {
            currentEvent++;
            StartCoroutine(PlayEvent());
        }
        else
        {
            done = true;
            ReloadScene();
        }

        if (journey[currentEvent].particles != null)
            journey[currentEvent].particles.Stop();
    }


    public void ReloadScene()
    {
        //TODO maybe change this so that it resets elements and lowers the map
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
