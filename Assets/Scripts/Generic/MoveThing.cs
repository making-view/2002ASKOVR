using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThing : MonoBehaviour
{
    [SerializeField]
    float duration = 20;

    [SerializeField]
    GameObject endposGO = null;

    Vector3 startPos;
    Vector3 endPos;

    [SerializeField]
    bool smooth = true;

    //private bool soundPlayed = false;

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

    IEnumerator Move(bool forward)
    {
        Vector3 startPoint = Vector3.zero;
        Vector3 endPoint = Vector3.zero;

        if (forward)
        {
            startPoint = startPos;
            endPoint = endPos;
        }
        else
        {
            startPoint = endPos;
            endPoint = startPos;
        }

        float timePassed = 0.0f;

        while (timePassed < duration)
        {
            if (!smooth)
            {
                this.transform.position = Vector3.Lerp(startPoint, endPoint, timePassed / duration);
            }
            else
            {
                this.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.SmoothStep(0, 1, timePassed / duration));
            }

            yield return null;
            timePassed += Time.deltaTime;
        }

        this.transform.position = endPoint;
    }

        //functions to play the movement
        public void Play(bool forward)
    {
        StartCoroutine(Move(forward));

        if (audiosource != null)
            audiosource.Play();
    }

    public void SnapBack()
    {
        //To reality, oh there goes gravity
        var prevGrav = Physics.gravity;
        Physics.gravity = Vector3.zero;

        this.transform.position = startPos;

        Physics.gravity = prevGrav;
    }
}
