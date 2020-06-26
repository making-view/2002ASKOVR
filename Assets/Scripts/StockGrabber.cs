using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockGrabber : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GestureDetector gestureDetector;
    [SerializeField] private GrabHandle grabHandle;

    [Header("Settings")]
    [SerializeField] private float snatchTime = 0.3f;

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
        if (grabbedStock == null && focusedStock != null && gestureDetector.IsGestureActive(PoseName.Fist))
            GrabBegin();

        if (grabbedStock != null && !gestureDetector.IsGestureActive(PoseName.Fist))
            GrabEnd();
    }

    //
    // Called by RayTool through HandsManager when it targets an object of Stock type
    //
    public void SetFocusOnStock(Stock stock)
    {
        if (grabbedStock == null)
            focusedStock = stock;
    }

    //
    // Called by RayTool through HandsManager when it defocuses an object of any type
    //
    public void DeFocus()
    {
        focusedStock = null;
    }

    //
    // Prepares attachment between Stock and StockGrabber, starts displaying handle
    //
    private void GrabBegin()
    {
        grabbedStock = focusedStock;

        var grabHeight = grabbedStock.GetComponent<BoxCollider>().size.y / 1.6f;

        grabHandle.gameObject.SetActive(true);

        StartCoroutine(SnatchStock(grabHeight));
        DeFocus();
    }

    //
    // Severs attachment by droping the grabbed Stock and hiding the handle
    //
    private void GrabEnd()
    {
        grabbedStock.Drop();
        grabbedStock = null;

        grabHandle.gameObject.SetActive(false);
    }

    //
    // Summons Stock from its position to line up with GrabHandle, 
    // then grabs it if StockGrabber hasn't already been told to drop it
    //
    private IEnumerator SnatchStock(float grabHeight)
    {
        var timer = 0.0f;
        var initialPos = grabbedStock.transform.position;

        while (grabbedStock != null && timer <= snatchTime)
        {
            var targetPos = grabHandle.stockHolder.transform.position - (Vector3.up * grabHeight);

            grabbedStock.transform.position = Vector3.Lerp(initialPos, targetPos, timer / snatchTime);

            yield return null;

            timer += Time.deltaTime;
        }

        if (grabbedStock != null)
            grabbedStock.Grab(grabHandle, grabHeight);
    }
}
