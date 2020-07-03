using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockHolderBender : MonoBehaviour
{
    [SerializeField] private GameObject firstPart = null;
    [SerializeField] private GameObject[] bendyParts = null;

    void Update()
    {
        transform.up = Vector3.up;

        var nonBendyParts = 2; // First and last
        var partCount = bendyParts.Length + nonBendyParts;
        var firstToLastDistance = (firstPart.transform.position - transform.position).magnitude;

        var p0 = firstPart.transform.position;
        var p1 = transform.position + (Vector3.up * (firstToLastDistance / 2));
        var p2 = transform.position;

        var firstRotation = firstPart.transform.rotation;
        var lastRotation = transform.rotation;

        for (int part = 0; part < bendyParts.Length; ++part)
        {
            //
            // +1 because zero-based index doesn't make sense for percentage calculation
            // +1 more because first part isn't going to be bent programmatically
            //
            var percent = (part + 2f) / partCount;

            bendyParts[part].transform.position = GetPointOnBezierCurve(p0, p1, p2, percent);
            bendyParts[part].transform.rotation = Quaternion.Lerp(firstRotation, lastRotation, percent);
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
}
