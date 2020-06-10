using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSkeleton))]
public class GestureTeleporter : MonoBehaviour
{
    public GestureDetector gestureDetector;
    public float rayLength = 10f;

    private OVRSkeleton skeleton;

    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();
    }

    void Update()
    {
        if (gestureDetector.IsGestureActive(PoseName.JazzHand))
        {
            Vector3 palmUp = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.right : transform.right;
            Vector3 rayPointDirection = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.up : transform.up;
            Vector3 start = transform.position + palmUp * 0.08f;
            Vector3 end = start + (rayPointDirection * rayLength) + (palmUp * (rayLength / 2));
        }
    }
}
