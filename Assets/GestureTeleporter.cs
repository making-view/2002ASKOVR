using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSkeleton))]
public class GestureTeleporter : MonoBehaviour
{
    public GestureDetector gestureDetector;

    private OVRSkeleton skeleton;
    private LineRenderer lineRenderer;

    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (gestureDetector.IsGestureActive(PoseName.JazzHand))
        {
            Vector3 handDirection = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.up : transform.up;
            Vector3 start = transform.position;
            Vector3 end = start + handDirection;
            
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new List<Vector3>() { start, end }.ToArray());
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
