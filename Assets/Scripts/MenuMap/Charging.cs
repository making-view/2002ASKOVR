using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles animations and effects for windmills and charging.
/// Place this script on cable and give refs to windmills and charger
/// </summary>
public class Charging : MonoBehaviour
{
    [SerializeField] float blinkSpeed = 4.0f;

    [SerializeField] Highlight windmill1 = null;
    [SerializeField] Highlight windmill2 = null;
    [SerializeField] Highlight charger = null;

    private Material cableMaterial = null;
    private List<Highlight> highlights = null;
    // Start is called before the first frame update
    void Start()
    {
        cableMaterial = GetComponent<Renderer>().material;

        var color = cableMaterial.GetColor("_Color0");
        cableMaterial.SetColor("_Color0", new Color(color.r, color.g, color.b, 0.0f));

        highlights = new List<Highlight>();

        highlights.AddRange(windmill1.GetComponentsInChildren<Highlight>());
        highlights.AddRange(windmill2.GetComponentsInChildren<Highlight>());

        //StartCharging(20.0f);
    }


    public void StartCharging(float duration)
    {
        StartCoroutine(Charge(duration));
    }

    //windmills start spinning. Shows flow of clean energy from windmill -> charging station
    IEnumerator Charge(float duration)
    {
        float step = duration / 2;


        foreach (Highlight h in highlights)
            h.StartHighlight(step);

        windmill1.gameObject.GetComponentInChildren<SpinBoi>().startSpin();
        windmill2.gameObject.GetComponentInChildren<SpinBoi>().startSpin();

        StartCoroutine(CableBlinking(step));

        yield return new WaitForSeconds(step);

        charger.StartHighlight(step);
    }

    IEnumerator CableBlinking(float time)
    {
        float timePassed = 0;
        var color = cableMaterial.GetColor("_Color0");

        while (timePassed < time)
        {
            var sinWave = (Mathf.Sin(timePassed * blinkSpeed) + 1) / 2;        
            cableMaterial.SetColor("_Color0", new Color(color.r, color.g, color.b, sinWave));
            yield return null;
            timePassed += Time.deltaTime;
        }

        cableMaterial.SetColor("_Color0", new Color(color.r, color.g, color.b, 1.0f));
    }
}
