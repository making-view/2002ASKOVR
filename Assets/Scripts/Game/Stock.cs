﻿using System.Collections;
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
    private float angleAdjustCooldownTime = 0.75f;
    private float angleAdjustVelocity = 550;

    private BoxCollider ownCollider;
    private Rigidbody rigidBody;

    private GrabHandle grabHandle;
    private float grabHeight = 0.0f;
    private float movementSensitivity = 0.01f;
    private float angleAdjustThreshold = 15.0f;
    private float angleAdjustCooldownTimer = 0.0f;
    private float stockHorizontalAngle = 0;


    private float previousHandleRoll = 0.0f;
    private float previousHandleYaw = 0.0f;

    private List<BoxCollider> otherColliders;

    void Start()
    {
        ownCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

        otherColliders = new List<BoxCollider>();
    }

    void Update()
    {
        angleAdjustCooldownTimer -= Time.deltaTime;

        //
        // If attached to a grabHandle, move along with it
        //
        if (grabHandle)
        {
            var handleForward = new Vector3(grabHandle.transform.forward.x, 0, grabHandle.transform.forward.z);
            var handleRoll = Vector3.SignedAngle(Vector3.up, grabHandle.transform.right, handleForward);
            var handleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);

            var forwardUpAngle = Vector3.Angle(Vector3.up, grabHandle.transform.forward);
            var canAdjustAngles = angleAdjustCooldownTimer <= 0.0f 
                && 90 - angleAdjustThreshold <= forwardUpAngle 
                && 90 + angleAdjustThreshold >= forwardUpAngle;

            if (canAdjustAngles)
            {
                var smallestRollChange = handleRoll - previousHandleRoll;
                smallestRollChange += smallestRollChange > 180 ? -360 : (smallestRollChange < -180) ? 360 : 0;
                var smallestYawChange = handleYaw - previousHandleYaw;
                smallestYawChange += smallestYawChange > 180 ? -360 : (smallestYawChange < -180) ? 360 : 0;

                var handleRollVelocity = smallestRollChange / Time.deltaTime;
                var handleYawVelocity = smallestYawChange / Time.deltaTime;

                if (Mathf.Abs(handleYawVelocity) >= angleAdjustVelocity)
                {
                    stockHorizontalAngle += Mathf.Sign(handleYawVelocity) * 90;
                    angleAdjustCooldownTimer = angleAdjustCooldownTime;
                }
            }

            var lastCoilTrans = grabHandle.lastSpringCoil.transform;
            var newRot = Quaternion.Euler(new Vector3(0, stockHorizontalAngle, 0));
            var newPos = lastCoilTrans.position - (lastCoilTrans.up * grabHeight);

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

            previousHandleRoll = handleRoll;
            previousHandleYaw = handleYaw;
        }
    }

    //
    // Set up attachment of self to the handle of the StockGrabber grabbing this object
    //
    public void Grab(GrabHandle handle, float height, float yAngle)
    {
        rigidBody.isKinematic = true;

        grabHandle = handle;
        grabHeight = height;
        stockHorizontalAngle = yAngle;

        var handleForward = new Vector3(grabHandle.transform.forward.x, 0, grabHandle.transform.forward.z);
        previousHandleRoll = Vector3.SignedAngle(Vector3.up, handle.transform.right, handleForward);
        previousHandleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);
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
