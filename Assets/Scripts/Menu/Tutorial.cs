using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public enum Task
    {
        Teleport,
        Grab,
        RotateStock,
        Stack,
        EnterCommand,
        None
    }

    public bool IsTutorialOngoing { get; private set; } = false;

    [Serializable]
    private class Event
    {
        [SerializeField] public string name = "default";
        [SerializeField] public Task task = Task.None;
        [SerializeField] public AudioClip narration = null;
        [SerializeField] public int numberOfTimes = 4;
        [SerializeField] public float delayOnComplete = 2.0f;

        [SerializeField] public UnityEvent onStartOfEvent = null;
    }

    private int eventIndex = 0;
    [SerializeField] private List<Event> events = null;
    private bool waitingForNextEvent = true;
    private AudioSource audioSource = null;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (events.Count <= 0)
            throw new SystemException("haha, fuck you! CIA niggers \n\t\t\t\t\t\t-" + name);
    }

    public void StartPopupshit()
    {
        eventIndex = 0;
        waitingForNextEvent = true;
        IsTutorialOngoing = true;
        StartCoroutine(StartEvent(eventIndex));
    }

    public bool DoTask(Task action)
    {
        var didTask = false;

        if (!waitingForNextEvent && eventIndex < events.Count)
        {
            didTask = true;

            if (events[eventIndex].task.Equals(action))
                events[eventIndex].numberOfTimes--;

            if (events[eventIndex].numberOfTimes <= 0)
            {
                waitingForNextEvent = true;
                StartCoroutine(StartEvent(eventIndex));
            }
        }

        return didTask;
    }

    private IEnumerator StartEvent(int newIndex)
    {
        //if donzo
        if (newIndex >= events.Count)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
        else
        {
            if (newIndex > 0)
            {
                Debug.Log("starting task " + events[newIndex].name + " in " + events[newIndex - 1].delayOnComplete + " seconds after audio");
                yield return new WaitForSeconds(events[newIndex - 1].delayOnComplete);
            }

            if (events[newIndex].narration != null)
            {
                audioSource.clip = events[newIndex].narration;
                audioSource.Play();
            }

            events[newIndex].onStartOfEvent.Invoke();

            while (audioSource.isPlaying)
                yield return null;

            waitingForNextEvent = false;

            if (++eventIndex >= events.Count)
                SceneManager.LoadScene("Scenes/Menu");

            DoTask(Task.None);
        }
    }
}
