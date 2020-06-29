using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

[RequireComponent(typeof(ButtonController))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Stock : MonoBehaviour
{
    // How long the object has to be stationary before its physics are turned off
    [SerializeField] private float stationaryTime = 1.0f;

    private BoxCollider boxCollider;
    private Rigidbody rigidBody;

    private GrabHandle grabHandle;
    private float stockGrabbedRot = 0.0f;
    private float handleGrabbedRot = 0.0f;
    private float grabHeight = 0.0f;
    private float movementSensitivity = 0.01f;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //
        // If attached to a grabHandle, move along with it
        //
        if (grabHandle != null)
        {
            var newRot = stockGrabbedRot + (grabHandle.transform.rotation.eulerAngles.y - handleGrabbedRot);

            transform.position = grabHandle.stockHolder.transform.position - (Vector3.up * grabHeight);
            transform.rotation = Quaternion.Euler(new Vector3(0, newRot, 0));
        }
    }

    //
    // Set up attachment of self to the handle of the StockGrabber grabbing this object
    //
    public void Grab(GrabHandle handle, float height)
    {
        rigidBody.isKinematic = true;

        grabHandle = handle;
        grabHeight = height;
        stockGrabbedRot = transform.rotation.eulerAngles.y;
        handleGrabbedRot = grabHandle.transform.rotation.eulerAngles.y;
    }

    //
    // Remove reference to grabPoint, enable physics and start coroutine to eventually stop physics again
    //
    public void Drop()
    {
        rigidBody.isKinematic = false;

        grabHandle = null;
        StartCoroutine(DisableGravityOnceStationary());
    }

    private void OnCollisionEnter(Collision collision)
    {
        var stock = collision.gameObject.GetComponent<Stock>();


    }

    private void OnCollisionExit(Collision collision)
    {

    }

    //
    // Turns off physics after object has been stationary for some time
    //
    private IEnumerator DisableGravityOnceStationary()
    {
        var timer = 0.0f;
        var previousPosition = Vector3.zero;

        while (timer < stationaryTime)
        {
            timer += Time.deltaTime;

            var diff = transform.position - previousPosition;

            if (diff.magnitude > movementSensitivity) 
                timer = 0.0f;

            previousPosition = transform.position;

            yield return null;
        }

        rigidBody.isKinematic = true;
    }
}
