using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StockGrabber : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected GrabHandle grabHandle = null;

    [Header("Settings")]
    [SerializeField] protected float snatchTime = 0.3f;
    [SerializeField] protected float additionalFloatDistance = 0.0f;

    protected Stock focusedStock = null;
    protected Stock grabbedStock = null;

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
    protected void GrabBegin()
    {
        //
        // Only grab stock if there are no stock items on top of it
        //
        if (focusedStock.GetOverheadStock().Count == 0)
        {
            grabbedStock = focusedStock;

            var stockCollider = grabbedStock.GetComponent<BoxCollider>();
            // COULD DO: Finn lengste side(r) og gjør det bedre
            var floatDistance = ((stockCollider.size.x + stockCollider.size.y) / 2) + additionalFloatDistance;

            grabHandle.gameObject.SetActive(true);

            StartCoroutine(SnatchStock(floatDistance));
            DeFocus();
        }
    }

    //
    // Severs attachment by droping the grabbed Stock and hiding the handle
    //
    protected void GrabEnd()
    {
        grabbedStock.Drop();
        grabbedStock = null;

        grabHandle.gameObject.SetActive(false);
    }

    //
    // Summons Stock from its position to line up with GrabHandle, 
    // then grabs it if StockGrabber hasn't already been told to drop it
    //
    private IEnumerator SnatchStock(float floatDistance)
    {
        var timer = 0.0f;
        var initialPos = grabbedStock.transform.position;
        var initialRot = grabbedStock.transform.rotation;

        var closestXRightAngle = Mathf.Round(initialRot.eulerAngles.x / 90) * 90;
        var closestYRightAngle = Mathf.Round(initialRot.eulerAngles.y / 90) * 90;
        var closestZRightAngle = Mathf.Round(initialRot.eulerAngles.z / 90) * 90;
        var targetRot = Quaternion.Euler(new Vector3(closestXRightAngle, closestYRightAngle, closestZRightAngle));

        while (grabbedStock != null && timer <= snatchTime)
        {
            var lastCoil = grabHandle.lastSpringCoil.transform;
            var percent = timer / snatchTime;
            var targetPos = lastCoil.position - (lastCoil.up * floatDistance);

            grabbedStock.transform.position = Vector3.Lerp(initialPos, targetPos, percent);
            grabbedStock.transform.rotation = Quaternion.Lerp(initialRot, targetRot, percent);

            yield return null;

            timer += Time.deltaTime;
        }

        if (grabbedStock != null)
            grabbedStock.Grab(this, grabHandle, floatDistance, (int)closestXRightAngle, (int)closestYRightAngle, (int)closestZRightAngle);
    }
}
