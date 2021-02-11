using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles animations and effects for windmills and charging.
/// Place this script on cable and give refs to windmills and charger
/// </summary>
public class Charging : MonoBehaviour
{
    [SerializeField] GameObject windmill1 = null;
    [SerializeField] GameObject windmill2 = null;
    [SerializeField] GameObject charger = null;

    private Material cableMaterial = null;
    // Start is called before the first frame update
    void Start()
    {
        cableMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        var sinWave = (Mathf.Sin(Time.timeSinceLevelLoad * 5) + 1) / 2;


        var color = cableMaterial.GetColor("_Color0");
        cableMaterial.SetColor("_Color0", new Color(color.r, color.g, color.b, sinWave));
    }
}
