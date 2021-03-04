using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private OVRScreenFade fader = null;

    public void LoadGame(string scene)
    {
        StartCoroutine(FadeAndLoad(scene));
    }

    IEnumerator FadeAndLoad(string scene)
    {
        fader.fadeTime = 1.0f;
        fader.FadeOut();

        yield return new WaitForSeconds(1.0f);

        var operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = true;
    }
}
