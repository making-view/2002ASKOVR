
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


    // Start is called before the first frame update
    void Start()
    {

        mediaPlayer = GetComponent<VideoPlayer>();

        var extension = ".mp4";

        if (Application.platform != RuntimePlatform.Android)
        {
            var root = "D:/Ressurser/Video/ASKO/";
            mediaPlayer.url = root + fileName + extension;
        }
        else
        {
            var path = Path.Combine(Application.persistentDataPath, "Movies");
            mediaPlayer.url = Path.Combine(path, fileName + extension);
        }
    }

    public void StopVideo()
    {
        mediaPlayer.Stop();
    }

    public void StartVideo()
    {
        mediaPlayer.Play();
    }
}
