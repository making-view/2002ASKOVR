using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThing : MonoBehaviour
{
    [SerializeField]
    float duration = 20;

    [SerializeField]
    GameObject endposGO;

    Vector3 startPos;
    Vector3 endPos;

    [SerializeField]
    bool smooth = false;

    [SerializeField]
    private bool started = false;
    private bool soundPlayed = false;
    private float timeElapsed = 0;

    [SerializeField]
    AudioClip audioclip = null;

    AudioSource audiosource = null;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.gameObject.transform.position;
        endPos = endposGO.transform.position;

        audiosource = gameObject.AddComponent<AudioSource>();
        audiosource.clip = audioclip;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            timeElapsed += Time.deltaTime;

            if (!(timeElapsed > duration))
            {
                if (!smooth)
                {
                    this.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
                }
                else
                {
                    this.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, timeElapsed / duration));
                }
            }
            else
            {
                started = false;
            }
        }
    }

        //functions to play the movement
        public void Play()
    {
        this.transform.position = startPos;
        started = true;
        timeElapsed = 0;

        if (audiosource != null)
            audiosource.Play();
    }

    public void GoBack()
    {
        started = false;
        this.transform.position = startPos;
    }
}
