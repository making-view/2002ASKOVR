using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.IO;

public class VRVideoTrigger : MonoBehaviour
{

    [SerializeField] private GameObject videoSphere = null;
    [SerializeField] private GameObject surroundings = null;

    [SerializeField] private MediaPlayer mediaPlayer = null;
    [SerializeField] private string fileName = null;


    private string url;
    private bool fading = false;

    public bool IsVideoPlaying
    {
        get
        {
            return !GetComponent<MeshRenderer>().enabled;
        }
    }

    void Start()
    {
        var extension = ".mp4";

        if (Application.platform != RuntimePlatform.Android)
        {
            var root = "D:/Ressurser/Video/ASKO/";
            url = root + fileName + extension;
        }
        else
        {
            var path = Path.Combine(Application.persistentDataPath, "Movies");
            url = Path.Combine(path, fileName + extension);
        }

        Debug.Log("video set: " + gameObject.name + " - " + url);

        mediaPlayer.Events.AddListener(OnVideoEvent);

    }

    public void Activate()
    {
        if (!fading)
            StartCoroutine(FadeToVideo());
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                StopVideo();
                break;
        }
    }

    public void StopVideo()
    {
        if (!fading)
            StartCoroutine(FadeFromVido());
    }

    IEnumerator FadeToVideo()
    {

        fading = true;

        var fade = FindObjectOfType<OVRScreenFade>();

        fade.fadeColor = Color.black;
        fade.fadeTime = 1.0f;
        fade.FadeOut();

        yield return new WaitForSeconds(1.1f);

        //video notication removed from here

        videoSphere.SetActive(true);
        surroundings.SetActive(false);

        foreach (var renderer in transform.parent.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, url, true);

        while (!mediaPlayer.Control.IsPlaying())
            yield return null;

        fade.FadeIn();

        yield return new WaitForSeconds(1.1f);

        fading = false;
    }

    IEnumerator FadeFromVido()
    {
        fading = true;

        var fade = FindObjectOfType<OVRScreenFade>();

        fade.fadeColor = Color.black;
        fade.fadeTime = 1.0f;
        fade.FadeOut();

        yield return new WaitForSeconds(1.1f);

        videoSphere.SetActive(false);
        surroundings.SetActive(true);

        foreach (var renderer in transform.parent.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = true;
        }

        mediaPlayer.Stop();

        fade.FadeIn();

        yield return new WaitForSeconds(1.1f);

        fading = false;
    }
}


