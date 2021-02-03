using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        public MoveThing moveThing = null;
        public AudioClip narration = null;
        public ParticleSystem particles = null;
        public UnityEvent actions = null;

    }

    private BezierSolution.BezierSqauencer sequencer;
    private AudioSource narrator;

    [SerializeField] List<JourneyEvent> journey = null;
    private int currentEvent = 0;
    private float nextEvent = 5.0f;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        narrator = GetComponent<AudioSource>();
    }

    public void BeginPlay()
    {
        if(done)
        {
            Debug.Log(gameObject.name + " should reset scene completely or manually. TODO");
        }


        StopCoroutine(PlayEvent());
        StopEvent();

        currentEvent = 0;
        if (journey.Count > currentEvent)
            StartCoroutine(PlayEvent());
    }


    public void BeginPlay(int eventnum)
    {
        StopCoroutine(PlayEvent());
        StopEvent();

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
        nextEvent = journey[currentEvent].duration;

        if (journey[currentEvent].animation != null)
            journey[currentEvent].animation.Play();

        if (journey[currentEvent].moveThing != null)
            journey[currentEvent].moveThing.Play();

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
        }

        if (journey[currentEvent].particles != null)
            journey[currentEvent].particles.Stop();
    }
}
