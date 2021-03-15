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
    [SerializeField] GameObject mapMenu = null;
    [SerializeField] List<JourneyEvent> journey = null;

    private int currentEvent = 0;
    private int targetEvent = 0;
    private bool initialized = false;
    private int nEvents = 1;
    private float nextEvent = 5.0f;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        narrator = GetComponent<AudioSource>();
        //targetEvent = journey.Count + 1;
        targetEvent = GetEventnum("ExplorationPause");
        Debug.Log("target event set to: " + targetEvent);

        initialized = true;
    }

    private void Update()
    {
        mapMenu.SetActive(targetEvent <= currentEvent);
    }

    //Sets the amount of events to play during PlayFrom(int i) mode
    public void SetEventGap(int nEvents)
    {
        this.nEvents = nEvents;
    }

    public void PlayFrom(string eventName)
    {
        StopCoroutine(PlayEvent());
        StopEvent();

        currentEvent = GetEventnum(eventName);

        if (eventName.Equals("LowerMap"))
        {
            Debug.LogWarning("I'm a dirty fraud");
            targetEvent = journey.Count;
        }

        targetEvent = currentEvent + nEvents;

        if (currentEvent <= targetEvent && targetEvent < journey.Count + 1)
            StartCoroutine(PlayEvent());
        else
            Debug.LogError("tried playing event outside range. " + this.gameObject.name);
    }


    public void BeginPlay(int eventnum)
    {
        StopAllCoroutines();
        StopCoroutine(PlayEvent());
        StopEvent();

        currentEvent = eventnum;

        if (journey.Count > currentEvent + 1)
            StartCoroutine(PlayEvent());
    }

    public void BeginPlay(string eventname)
    {
        StopAllCoroutines();
        StopCoroutine(PlayEvent());
        StopEvent();

        int eventnum = GetEventnum(eventname);

        currentEvent = eventnum;
        StartCoroutine(PlayEvent());
    }

    private int GetEventnum(string eventname)
    {
        int eventnum = 0;

        for (int i = 0; i < journey.Count; i++)
        {
            if (journey[i].name.Equals(eventname))
            {
                eventnum = i;
                break;
            }
        }
        if (eventnum == 0)
            Debug.Log("event " + eventname + " not found or first");
        return eventnum;
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
        while (!initialized)
            yield return null;

        Debug.Log("playing event: " + journey[currentEvent].name + "\nTarget event: " + targetEvent);

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
            narrator.PlayOneShot(journey[currentEvent].narration);

        journey[currentEvent].actions.Invoke();

        yield return new WaitForSeconds(nextEvent);

        if (targetEvent > currentEvent)
        {
            Debug.Log("playing next event");
            currentEvent++;
            StartCoroutine(PlayEvent());
        }
        else
        {
            done = true;
            mapMenu.SetActive(true);
            //make exploration or reset available?
            //ReloadScene();
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
