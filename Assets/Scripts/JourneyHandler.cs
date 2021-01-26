using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public EventType type;
        public Animator animator = null;
        public MoveThing moveThing = null;
        public AudioClip narration = null;

        public float timeToNext = 1f; 
    }

    private BezierSolution.BezierSqauencer sequencer;
    private AudioSource narrator;

    [SerializeField] List<JourneyEvent> journey;
    private int currentEvent = 0;
    private float nextEvent = 5.0f;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        BeginPlay();
    }

    void BeginPlay()
    {
        currentEvent = 0;
        if (journey.Count > currentEvent)
            StartCoroutine(NextEvent(nextEvent));
    }

    private void PlayEvent()
    {
        nextEvent = journey[currentEvent].timeToNext;

        if (journey[currentEvent].animator != null)
            journey[currentEvent].animator.Play(0);

        if (journey[currentEvent].moveThing != null)
            journey[currentEvent].moveThing.Play();

        if (journey[currentEvent].narration != null)
        {
            narrator.PlayOneShot(journey[currentEvent].narration);
            nextEvent += narrator.clip.length;
        }


        if (journey.Count > currentEvent + 1)
        {
            currentEvent++;
            StartCoroutine(NextEvent(nextEvent));
        }
    }

    private IEnumerator NextEvent(float time)
    {
        yield return new WaitForSeconds(time);
        PlayEvent();
    }
}
