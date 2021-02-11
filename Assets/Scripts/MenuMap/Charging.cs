using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles animations and effects for windmills and charging.
/// Place this script on cable and give refs to windmills and charger
/// </summary>
public class Charging : MonoBehaviour
{
    [SerializeField] float speed = 4.0f;

    [SerializeField] Highlight windmill1 = null;
    [SerializeField] Highlight windmill2 = null;
    [SerializeField] Highlight charger = null;

    private Material cableMaterial = null;
    // Start is called before the first frame update
    void Start()
    {
        cableMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //ladekabel pulserer
        var sinWave = (Mathf.Sin(Time.timeSinceLevelLoad * speed) + 1) / 2;
        var color = cableMaterial.GetColor("_Color0");
        cableMaterial.SetColor("_Color0", new Color(color.r, color.g, color.b, sinWave));
    }


    IEnumerator Charge()
    {
        //TODO vindmøller begynner blinke og spinner opp gradvis
        yield return new WaitForSeconds(0.5f);
        //TODO Ladekabel fader inn og pulserer
        yield return new WaitForSeconds(0.5f);
        //TODO Ladestasjon lyser opp ved ASKO
        yield return new WaitForSeconds(0.5f);
        //TODO fade ut lys
        yield return new WaitForSeconds(0.5f);
        //TODO gjør ladekabel usynlig
    }
}
