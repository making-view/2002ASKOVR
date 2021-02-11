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

        material = GetComponent<Renderer>().material;
        originalColor = material.GetColor("_EmissionColor");
    }

    public void StartHighlight(float time)
    {
        StopCoroutine("HighLightAsync");
        StartCoroutine(HighLightAsync(time));
    }


    IEnumerator HighLightAsync(float time)
    {
        if (spotlight != null)
            spotlight.enabled = true;

        float timePassed = 0.0f;

        while(timePassed < time)
        {
            float lerpyboi = (Mathf.Sin(timePassed * speed) + 1) / 2;

            if (changeColor)
            {
                Color newColor = Color.Lerp(downlightColor, highlightColor, lerpyboi);
                material.SetColor("_EmissionColor", newColor);
            }

            if(spotlight != null)
                spotlight.intensity = Mathf.Lerp(0, intensity, lerpyboi);

            yield return null;

            timePassed += Time.deltaTime;
        }

        material.SetColor("_EmissionColor", originalColor);

        if (spotlight != null)
            spotlight.enabled = false;
    }
}
