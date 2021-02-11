using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Satellite : MonoBehaviour
{
    [SerializeField] GameObject ASKOLazor;
    [SerializeField] GameObject OtherLazor;
    private Renderer askomaterial;
    private Renderer kiwimaterial;

    private AudioSource audioSource = null;
    private LookAtConstraint lazorConstraint = null;
    [Range(0.1f, 20)] [SerializeField] float speed = 10.0f;

    private void Start()
    {
        lazorConstraint = OtherLazor.GetComponentInParent<LookAtConstraint>();

        audioSource = GetComponent<AudioSource>();
        askomaterial = ASKOLazor.GetComponent<Renderer>();
        kiwimaterial = OtherLazor.GetComponent<Renderer>();

        ActivateLazors(false);

        sendData(5.0f);

        ToggleTarget();
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
        OtherLazor.SetActive(activate);
    }

    public void ToggleTarget()
    {
        List<ConstraintSource> newSources = new List<ConstraintSource>();
        lazorConstraint.GetSources(newSources);

        List<ConstraintSource> newnewSourcesFuckYourShitModdafuckerREEEEeeeeee = new List<ConstraintSource>();

        for (int i = 0; i < newSources.Count; i++)
        {
            ConstraintSource newSource = newSources[i];
            newSource.weight = Mathf.Abs(newSources[i].weight - 1);

            newnewSourcesFuckYourShitModdafuckerREEEEeeeeee.Add(newSource);
        }

        lazorConstraint.SetSources(newnewSourcesFuckYourShitModdafuckerREEEEeeeeee);
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
