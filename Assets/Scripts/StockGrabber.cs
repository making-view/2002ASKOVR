using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockGrabber : MonoBehaviour
{
    [SerializeField] private GestureDetector gestureDetector;
    [SerializeField] private GameObject grabHandle;

    private Stock focusedStock = null;
    private Stock grabbedStock = null;

    public OVRSkeleton.SkeletonType SkeletonType
    {
        get
        {
            return gestureDetector.skeleton.GetSkeletonType();
        }
    }

    void Update()
    {
        if (focusedStock != null && gestureDetector.IsGestureActive(PoseName.Fist))
        {
            grabbedStock = focusedStock;

            grabHandle.SetActive(true);
            DeFocus();
        }

        if (grabbedStock != null && !gestureDetector.IsGestureActive(PoseName.Fist))
        {
            grabbedStock = null;

            grabHandle.SetActive(false);
        }
    }

    public void SetFocusOnStock(Stock stock)
    {
        if (grabbedStock == null)
            focusedStock = stock;
    }

    public void DeFocus()
    {
        focusedStock = null;
    }
}
