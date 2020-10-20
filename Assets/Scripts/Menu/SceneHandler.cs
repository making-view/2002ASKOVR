using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private OVRScreenFade fader = null;

    public void LoadGame(Text text)
    {
        text.text = "Laster...";

        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        fader.fadeTime = 1.0f;
        fader.FadeOut();

        yield return new WaitForSeconds(1.0f);

        var operation = SceneManager.LoadSceneAsync("Scenes/Warehouse");
        operation.allowSceneActivation = true;
    }
}
