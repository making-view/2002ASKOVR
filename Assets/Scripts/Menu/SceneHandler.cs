using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public void LoadGame(Text text)
    {
        text.text = "Laster...";
        var operation = SceneManager.LoadSceneAsync("Scenes/Warehouse");
        operation.allowSceneActivation = true;
    }
}
