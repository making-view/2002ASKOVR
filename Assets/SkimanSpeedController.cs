using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interpolates the speed of a bezier walker based on height above bottom point relative to top point.
/// Thing go down hill go faster
/// </summary>
[RequireComponent(typeof(BezierSolution.BezierWalkerWithSpeed))]
public class SkimanSpeedController : MonoBehaviour
{

    [SerializeField] GameObject topPoint = null;
    [SerializeField] GameObject bottomPoint = null;
    [SerializeField] float minSpeed = 0.1f;
    [SerializeField] float maxSpeed = 0.8f;

    BezierSolution.BezierWalkerWithSpeed bezierWalker = null;
    ParticleSystem snowyParticles = null;

    private void Start()
    {
        bezierWalker = GetComponent<BezierSolution.BezierWalkerWithSpeed>();
        snowyParticles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var Hdiff = topPoint.transform.position.y - bottomPoint.transform.position.y;
        var height = (transform.position.y - bottomPoint.transform.position.y) / Hdiff;

        //Debug.Log("height variable: " + height);

        var newSpeed = Mathf.Lerp(maxSpeed, minSpeed, height);

        //if going over half speed add snow
        if (!snowyParticles.isPlaying && newSpeed > (maxSpeed - minSpeed) / 2)
            snowyParticles.Play();
        else if (snowyParticles.isPlaying && !(newSpeed > (maxSpeed - minSpeed) / 2))
            snowyParticles.Stop();

        bezierWalker.speed = newSpeed * newSpeed;
    }
}
