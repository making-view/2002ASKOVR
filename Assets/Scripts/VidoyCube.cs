using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.IO;

public class VidoyCube : MonoBehaviour
{

    [SerializeField] private GameObject videoSphere = null;
    [SerializeField] private GameObject tower = null;
    [SerializeField] private MediaPlayer mediaPlayer = null;
    [SerializeField] private string fileName = null;

    private string url;
    private bool fading = false;
    //private LangyAudio langyAudio = null;

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
            var root = "D:/Ressurser/Video/Koie/";
            url = root + fileName + extension;
        }
        else
        {
            var path = Path.Combine(Application.persistentDataPath, "Movies");
            url = Path.Combine(path, fileName + extension);
        }

        Debug.Log("video set: " + gameObject.name + " - " + url);

        mediaPlayer.Events.AddListener(OnVideoEvent);
        //mediaPlayer.m_Volume = 0;
        //langyAudio = FindObjectOfType<LangyAudio>();

    }

    public void Activate()
    {
        if (!fading && GetComponent<MeshRenderer>().enabled)
            StartCoroutine(FadeToVido());
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                StopVoidodsayu();
                break;
        }
    }

    public void StopVoidodsayu()
    {
        if (!fading)
            StartCoroutine(FadeFromVido());
    }

    IEnumerator FadeToVido()
    {

        fading = true;

        var fade = FindObjectOfType<OVRScreenFade>();

        fade.fadeColor = Color.black;
        fade.fadeTime = 1.0f;
        fade.FadeOut();

        yield return new WaitForSeconds(1.1f);

        //langyAudio.No();
        //FindObjectOfType<VideoNotification>().ShowNotification();


        videoSphere.SetActive(true);
        tower.SetActive(false);

        foreach (var renderer in transform.parent.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        //mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL,
            //url, true);

        while (!mediaPlayer.Control.IsPlaying())
            yield return null;

        fade.FadeIn();

        //langyAudio.PlayAduio(fileName);

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

        //langyAudio.No();

        videoSphere.SetActive(false);
        tower.SetActive(true);

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


