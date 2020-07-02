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

    private BoxCollider ownCollider;
    private Rigidbody rigidBody;

    private GrabHandle grabHandle;
    private float stockGrabbedRot = 0.0f;
    private float handleGrabbedRot = 0.0f;
    private float grabHeight = 0.0f;
    private float movementSensitivity = 0.01f;

    private List<BoxCollider> otherColliders;

    void Start()
    {
        ownCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

        otherColliders = new List<BoxCollider>();
    }

    void Update()
    {
        //
        // If attached to a grabHandle, move along with it
        //
        if (grabHandle)
        {
            var handleRotDifference = grabHandle.transform.rotation.eulerAngles.y - handleGrabbedRot;
            var rotationIncrement = Mathf.Round(handleRotDifference / 90) * 90;
            var newYAngle = stockGrabbedRot + rotationIncrement;

            var newRot = Quaternion.Euler(new Vector3(0, newYAngle, 0));
            var newPos = grabHandle.stockHolder.transform.position - (Vector3.up * grabHeight);

            Vector3 direction = Vector3.zero;
            float distance = 0.0f;

            foreach (var collider in otherColliders)
            {
                if (Physics.ComputePenetration(ownCollider, newPos, newRot, collider,
                    collider.gameObject.transform.position, collider.gameObject.transform.rotation,
                    out direction, out distance))
                {
                    newPos += direction * distance;
                }
            }

            transform.position = newPos;
            transform.rotation = newRot;
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
        if (grabHandle && collision.collider is BoxCollider)
        {
            otherColliders.Add(collision.collider as BoxCollider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider is BoxCollider && otherColliders.Contains(collision.collider as BoxCollider))
        {
            otherColliders.Remove(collision.collider as BoxCollider);
        }
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
