using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockGrabber : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GestureDetector gestureDetector = null;
    [SerializeField] private GrabHandle grabHandle = null;

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

        var grabHeight = grabbedStock.GetComponent<BoxCollider>().size.y;

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
        var initialRot = grabbedStock.transform.rotation;

        var closestRightAngle = Mathf.Round(initialRot.eulerAngles.y / 90) * 90;
        var targetRot = Quaternion.Euler(new Vector3(0, closestRightAngle, 0));

        while (grabbedStock != null && timer <= snatchTime)
        {
            var percent = timer / snatchTime;
            var targetPos = grabHandle.lastSpringCoil.transform.position - (Vector3.up * grabHeight);

            grabbedStock.transform.position = Vector3.Lerp(initialPos, targetPos, percent);
            grabbedStock.transform.rotation = Quaternion.Lerp(initialRot, targetRot, percent);

            yield return null;

            timer += Time.deltaTime;
        }

        if (grabbedStock != null)
            grabbedStock.Grab(grabHandle, grabHeight, closestRightAngle);
    }
}
