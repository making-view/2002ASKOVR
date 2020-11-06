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
        NumberKey,
        RepeatKey,
        ConfirmKey,
        BackspaceKey,
        None
    }

    private int[] scale = new int[] { 0, 2, 4, 5, 7, 9, 11, 12 };

    public bool IsTutorialOngoing { get; private set; } = false;
    public ButtonType? HighlightKey { get; private set; } = null;
    public bool HighlightStockCode { get; set; } = false;
    public bool HighlightStockAmount { get; set; } = false;

    public void ParseHighlightKey(string buttonType)
    {
        HighlightKey = Enum.TryParse(
            buttonType, out ButtonType highlightType) ? highlightType : (ButtonType?)null;
    }

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

    private int maxTask = 0;
    private int eventIndex = 0;
    [SerializeField] private List<Event> events = null;
    public bool waitingForNextEvent = true;
    private AudioSource narrationSource = null;
    private AudioSource feedbackSource = null;
    [SerializeField] private AudioClip feedback = null;

    // Start is called before the first frame update
    private void Start()
    {
        narrationSource = gameObject.AddComponent<AudioSource>();
        feedbackSource = gameObject.AddComponent<AudioSource>();
        feedbackSource.clip = feedback;
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
            {
                feedbackSource.Stop();
                events[eventIndex].numberOfTimes--;

                float step = 0.0f;
                if (maxTask == 1)
                    step = 0;
                else
                    step = (maxTask - 1.0f - events[eventIndex].numberOfTimes) / (maxTask - 1.0f);
                
                //step goes from 0 to 1 based on tasks done
                var tone = Mathf.Pow(1.05946f, scale[Mathf.RoundToInt(step * 7)]);

                feedbackSource.pitch = tone;

                feedbackSource.Play();
            }

            if (events[eventIndex].numberOfTimes <= 0)
            {
                ++eventIndex;

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
            yield return new WaitForSeconds(events[newIndex - 1].delayOnComplete);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
        else
        {
            if (newIndex > 0)
                yield return new WaitForSeconds(events[newIndex - 1].delayOnComplete);

            if (events[newIndex].narration != null)
            {
                narrationSource.clip = events[newIndex].narration;
                narrationSource.Play();
            }

            events[newIndex].onStartOfEvent.Invoke();

            while (narrationSource.isPlaying)
                yield return null;

            maxTask = events[newIndex].numberOfTimes;

            waitingForNextEvent = false;

            DoTask(Task.None);
        }
    }
}
