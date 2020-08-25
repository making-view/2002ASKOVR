using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureStockGrabber : StockGrabber
{
    [Header("Gesture")]
    [SerializeField] private GestureDetector gestureDetector = null;

    public OVRSkeleton.SkeletonType SkeletonType
    {
        get
        {
            return gestureDetector.skeleton.GetSkeletonType();
        }
    }

    void Update()
    {
        if (grabbedStock == null && focusedStock != null && gestureDetector.IsGestureActive(PoseName.Fist))
            GrabBegin();

        if (grabbedStock != null && !gestureDetector.IsGestureActive(PoseName.Fist))
            GrabEnd();
    }
}
