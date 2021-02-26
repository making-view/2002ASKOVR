using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] bool changeColor = true;
    [SerializeField] Color highlightColor = Color.white;
    [SerializeField] Color downlightColor = Color.black;
    Color originalColor = Color.white;
    private Material material = null;

    [Range(1.0f, 10.0f)]
    [SerializeField] private float speed = 4.0f;

    private Light spotlight;
    private float intensity;

    /// <summary>
    /// This class highlights the _EmissionColor variable and child spotlight of an object for x amounts of seconds with StartHighlight(time)
    /// </summary>
    void Start()
    {
        spotlight = GetComponentInChildren<Light>();
        if (spotlight != null)
        {
            intensity = spotlight.intensity;
            spotlight.enabled = false;
        }

        if (changeColor)
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.GetColor("_EmissionColor");
        }
        //StartHighlight(5.0f);
    }

    public void StartHighlight(float time)
    {
        StopHighlight();
        StartCoroutine(HighLightAsync(time));
    }

    public void StartHighlight()
    {
        StopHighlight();
        StartCoroutine(HighLightAsync(999.0f));
    }

    public void StopHighlight()
    {
        StopCoroutine("HighLightAsync");
        if (changeColor)
            material.SetColor("_EmissionColor", originalColor);
    }

        IEnumerator HighLightAsync(float time)
    {
        //haha this trick make your time irrelevant, but does make transition nicer
        time = time % (Mathf.PI * speed) + Mathf.PI * 0.5f;
 

        //------------------------------Pulse of Color/Light------------------------------//
        float fadeOut = time / 4;

        if (spotlight != null)
            spotlight.enabled = true;

        float timePassed = 0.0f;
        Color newColor = originalColor;

        while (timePassed < time * 3 / 4)
        {
            float lerpyboi = (Mathf.Sin(timePassed * speed) + 1) / 2;

            if (changeColor)
            {
                newColor = Color.Lerp(downlightColor, highlightColor, lerpyboi);
                material.SetColor("_EmissionColor", newColor);
            }

            if(spotlight != null)
                spotlight.intensity = Mathf.Lerp(0, intensity, lerpyboi);

            yield return null;

            timePassed += Time.deltaTime;
        }

        //-------------------------------Linear Fade out before end-------------------------//
        timePassed = 0;
        float currIntensity = 0;
        if(spotlight != null)
            currIntensity = spotlight.intensity;

        Color currColor = newColor;

        while(timePassed < fadeOut)
        {
            if (spotlight != null)
                spotlight.intensity = Mathf.Lerp(currIntensity, 0, timePassed / fadeOut);

            if (changeColor)
            {
                newColor = Color.Lerp(currColor, originalColor, timePassed / fadeOut);
                material.SetColor("_EmissionColor", newColor);
            }

            yield return null;
            timePassed += Time.deltaTime;
        }

        if (changeColor)
            material.SetColor("_EmissionColor", originalColor);

        if (spotlight != null)
            spotlight.enabled = false;
    }
}
