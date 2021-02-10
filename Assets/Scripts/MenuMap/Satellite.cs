using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    [SerializeField] GameObject ASKOLazor;
    [SerializeField] GameObject KIWILazor;
    private Renderer askomaterial;
    private Renderer kiwimaterial;

    private AudioSource audioSource = null;

    [Range(0.1f, 20)] [SerializeField] float speed = 10.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        askomaterial = ASKOLazor.GetComponent<Renderer>();
        kiwimaterial = KIWILazor.GetComponent<Renderer>();

        ActivateLazors(false);

        sendData(5.0f);
    }

    public void sendData(float time)
    {
        StopCoroutine(sendingData());
        StartCoroutine(sendingData(time));
    }

    public void sendData(bool sending)
    {
        StopCoroutine(sendingData());
        ActivateLazors(sending);

        if (sending)
            StartCoroutine(sendingData());

    }

    private void ActivateLazors(bool activate)
    {
        ASKOLazor.SetActive(activate);
        KIWILazor.SetActive(activate);
    }

    IEnumerator sendingData(float time)
    {
        audioSource.volume = 0;
        audioSource.Play();
        ActivateLazors(true);

        var maxTime = time;

        while (time > 0.0f)
        {
            audioSource.volume = Mathf.Sin((time / maxTime) * Mathf.PI);
            //Debug.Log("volume: " + audioSource.volume);

            PulseMaterial(askomaterial.material, true);
            PulseMaterial(kiwimaterial.material, false);
            yield return null;
            time -= Time.deltaTime;
        }

        audioSource.Stop();
        ActivateLazors(false);
    }

    IEnumerator sendingData()
    {
        while (enabled)
        {
            PulseMaterial(askomaterial.material, true);
            PulseMaterial(kiwimaterial.material, false);
            yield return null;
        }
    }

    private void PulseMaterial(Material material, bool offset)
    {
        float sinWave;

        if (!offset)
            sinWave = Mathf.Sin((Time.timeSinceLevelLoad * speed) + 1) / 2;
        else
            sinWave = Mathf.Sin((Time.timeSinceLevelLoad * speed + Mathf.PI) + 1) / 2;


        var color = material.GetColor("_Color0");
        material.SetColor("_Color0", new Color(color.r, color.g, color.b, sinWave));
    }
}
