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
    [SerializeField] private string PC_root = "D:/";

    private string url;
    private bool fading = false;

    [Space] // Tools for moving player rig to face forward in video
    [SerializeField] private GameObject cameraRig;
    private bool isRig;
    private float previousRotation;
    [SerializeField] [Tooltip("starting point in video")] private Transform videoTransform;



    public bool IsVideoPlaying
    {
        get
        {
            return !GetComponent<MeshRenderer>().enabled;
        }
    }

    void Start()
    {
        if (videoTransform == null)
            videoTransform = transform;

        var extension = ".mp4";

        if (Application.platform != RuntimePlatform.Android)
        {
            var root = PC_root + "Ressurser/Video/ASKO/";
            url = root + fileName + extension;
        }
        else
        {
            var path = Path.Combine(Application.persistentDataPath, "Movies");
            url = Path.Combine(path, fileName + extension);
        }

        Debug.Log("video set: " + gameObject.name + " - " + url);

        //mediaPlayer.Events.AddListener(OnVideoEvent);

        CheckforRequiredComponents();
    }

    private void CheckforRequiredComponents()
    {
        //Get OVRPlayerController or camera
        if (cameraRig == null)
            cameraRig = FindObjectOfType<OVRPlayerController>().gameObject;

        if (cameraRig == null || !cameraRig.activeInHierarchy)
        {
            Debug.Log("Vr rig not active, moving parent of cam instead");
            cameraRig = FindObjectOfType<Camera>().gameObject.transform.parent.gameObject;
            isRig = false;
        }
    }

    public void Activate()
    {
        if (!fading)
            StartCoroutine(PlayVideo());
    }

    //public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    //{
    //    switch (et)
    //    {
    //        case MediaPlayerEvent.EventType.FinishedPlaying:
    //            StopVideo();
    //            break;
    //    }
    //}

    public void StopVideo()
    {
        if (!fading)
            StartCoroutine(FadeFromVido());
    }

    IEnumerator PlayVideo()
    {
        fading = true;

        var fade = cameraRig.GetComponentInChildren<OVRScreenFade>();

        fade.fadeColor = Color.black;
        fade.fadeTime = 1.0f;
        fade.FadeOut();

        yield return new WaitForSeconds(1.1f);
        //face forward in video
        RotateToVideo(videoTransform.rotation.eulerAngles.y);

        RenderSettings.fog = false;
        videoSphere.SetActive(true);
        surroundings.SetActive(false);

        mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, url, true);

        while (!mediaPlayer.Control.IsPlaying())
            yield return null;

        fade.FadeIn();

        yield return new WaitForSeconds(1.1f);

        fading = false;

        //wait for duration of video - duration of fades
        yield return new WaitForSeconds((float)mediaPlayer.Info.GetDuration() - 2.2f);


        StopVideo();
    }

    IEnumerator FadeFromVido()
    {
        fading = true;

        //var fade = FindObjectOfType<OVRScreenFade>();
        var fade = cameraRig.GetComponentInChildren<OVRScreenFade>();

        fade.fadeColor = Color.black;
        fade.fadeTime = 1.0f;
        fade.FadeOut();

        yield return new WaitForSeconds(1.1f);

        videoSphere.SetActive(false);
        surroundings.SetActive(true);

        mediaPlayer.Stop();
        RenderSettings.fog = true;
        fade.FadeIn();

        //move back to previous map position
        RotateToVideo(previousRotation);
        yield return new WaitForSeconds(1.1f);

        fading = false;
    }


    private void RotateToVideo(float newRot)
    {
        var camPos = cameraRig.GetComponentInChildren<Camera>().transform;
        previousRotation = camPos.transform.rotation.eulerAngles.y;

        var diff = newRot - previousRotation;

        cameraRig.transform.RotateAround(camPos.position, Vector3.up, diff);


        Debug.Log("new rotation: " + newRot);
        Debug.Log("previous rotation: " + previousRotation);
    }
}


