using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSkeleton))]
public class SlideGrabber : MonoBehaviour
{
    public OVRSkeleton Skeleton { get; private set; }

    private void Start()
    {
        Skeleton = GetComponent<OVRSkeleton>();
    }

    void Update()
    {
        
    }
}
