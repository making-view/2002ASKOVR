using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid())
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}
