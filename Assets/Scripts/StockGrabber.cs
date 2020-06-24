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

    private bool stockGrabbed = false;
    private float stockGrabbedRot = 0.0f;
    private float handleGrabbedRot = 0.0f;
    private float grabHeight = 0.0f;

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

        if (stockGrabbed)
        {
            var newRot = stockGrabbedRot + (grabHandle.transform.rotation.eulerAngles.y - handleGrabbedRot);

            grabbedStock.transform.position = grabHandle.stockHolder.transform.position - (Vector3.up * grabHeight);
            grabbedStock.transform.rotation = Quaternion.Euler(new Vector3(0, newRot, 0));
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

    private void GrabBegin()
    {
        grabbedStock = focusedStock;
        grabHeight = grabbedStock.GetComponent<BoxCollider>().size.y / 1.6f;

        grabHandle.gameObject.SetActive(true);

        StartCoroutine(SnatchStock());
        DeFocus();
    }

    private void GrabEnd()
    {
        grabbedStock = null;
        stockGrabbed = false;

        grabHandle.gameObject.SetActive(false);
    }

    private IEnumerator SnatchStock()
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
        {
            stockGrabbed = true;
            stockGrabbedRot = grabbedStock.transform.rotation.eulerAngles.y;
            handleGrabbedRot = grabHandle.transform.rotation.eulerAngles.y;
        }
    }
}
