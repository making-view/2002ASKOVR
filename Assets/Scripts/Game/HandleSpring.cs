using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSpring : MonoBehaviour
{
    [SerializeField] private GameObject firstCoil = null;
    [SerializeField] private GameObject[] curvedCoils = null;
    [SerializeField] private GameObject lastCoil = null;

    void Update()
    {
        var bottomRot = new Vector3(0, firstCoil.transform.rotation.eulerAngles.y, 0);
        lastCoil.transform.up = Vector3.up;
        lastCoil.transform.rotation = Quaternion.Euler(bottomRot);

        var nonCurvedCoils = 2; // First and last
        var partCount = curvedCoils.Length + nonCurvedCoils;
        var firstToLastDistance = (firstCoil.transform.position - lastCoil.transform.position).magnitude;

        var p0 = firstCoil.transform.position;
        var p1 = lastCoil.transform.position + (Vector3.up * (firstToLastDistance / 2));
        var p2 = lastCoil.transform.position;

        var firstRotation = firstCoil.transform.rotation;
        var lastRotation = lastCoil.transform.rotation;

        for (int part = 0; part < curvedCoils.Length; ++part)
        {
            //
            // Remaps from the range 1 to partCount to the range 0 to 1 to calculate percent
            // Part +1 to bring minimum to 1
            // Part +1 to skip first part
            //
            var percent = RemapRange((part + 2f), 1f, partCount, 0, 1);

            curvedCoils[part].transform.position = GetPointOnBezierCurve(p0, p1, p2, percent);
            curvedCoils[part].transform.rotation = Quaternion.Lerp(firstRotation, lastRotation, percent);
        }
    }

    // Gets t percent point along the Bezier curve between given 3 points
    Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        var oneMinusT = 1f - t;
        var oneMinusTSqr = oneMinusT * oneMinusT;
        var tSqr = t * t;

        return (p0 * oneMinusTSqr) + (p1 * 2 * oneMinusT * t) + (p2 * tSqr);
    }

    float RemapRange (float oldValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        var oldRange = oldMax - oldMin;
        var newRange = newMax - newMin;

        var newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;

        return newValue;
    }
}
