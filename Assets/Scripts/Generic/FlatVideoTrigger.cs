
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

//attach to the same GO as a video player to enable cross paltform mp4 video streaming
[RequireComponent(typeof(VideoPlayer))]
public class FlatVideoTrigger : MonoBehaviour
{

    [SerializeField] private string fileName = null;
    private VideoPlayer mediaPlayer = null;
    private string url;
    [SerializeField] private string pc_root = "D:/";

    private GameObject monitorMesh = null;
    private Vector3 startScale = Vector3.zero;

    public bool quitVideo = false;
    private float popupTime = 1f;


    // Start is called before the first frame update
    void Start()
    {
        monitorMesh = transform.parent.parent.transform.gameObject;
        startScale = monitorMesh.transform.localScale;
        monitorMesh.transform.localScale = Vector3.zero;

        mediaPlayer = GetComponent<VideoPlayer>();

        var extension = ".mp4";

        if (Application.platform != RuntimePlatform.Android)
        {
            var root = pc_root + "Ressurser/Video/ASKO/";
            mediaPlayer.url = root + fileName + extension;
        }
        else
        {
            var path = Path.Combine(Application.persistentDataPath, "Movies");
            mediaPlayer.url = Path.Combine(path, fileName + extension);
        }

        //StartCoroutine(StartVideoAsync());
    }

    public void StopVideo()
    {
        mediaPlayer.Stop();
        monitorMesh.GetComponent<Animation>().Play("MonitorAway");
    }

    public void StartVideo()
    {
        quitVideo = false;
        StartCoroutine(StartVideoAsync());
    }

    IEnumerator StartVideoAsync()
    {
        monitorMesh.GetComponent<Animation>().Play("MonitorAppear");

        yield return new WaitForSeconds(popupTime);

        mediaPlayer.Prepare();

        while(!mediaPlayer.isPrepared)
            yield return null;

        mediaPlayer.Play();


        while(mediaPlayer.isPlaying && !quitVideo)
        {
            yield return null;
        }

        StopVideo();
    }


    //DEBUG update:
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //        quitVideo = true;
    //}
}
