using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CarryingArea carryingArea = null;
    [SerializeField] private TruckLanes truckLanes = null;
    [SerializeField] private Transform localOffset = null;
    [SerializeField] private Camera playerCamera = null;

    [Header("Settings")]
    [SerializeField] private float moveThreshold = 1.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float minZ = 0.0f;
    [SerializeField] private float maxZ = 0.0f;

    private bool moving = false;
    private int currentLane = 0;

    //
    // Initiate currentLane to current position
    //
    private void Start()
    {
        currentLane = truckLanes.FindLaneClosestToPoint(transform.position);
    }

    //
    // Determines if truck should across or between lanes, then executes the required movement
    //
    void Update()
    {
        //
        // Do not move truck if it is already moving
        //
        if (!moving)
        {
            var closestLane = truckLanes.FindLaneClosestToPoint(playerCamera.transform.position);

            if (closestLane == 1)
            {
                var currentZPos = transform.position.z - localOffset.localPosition.z;
                var zDiff = playerCamera.transform.position.z - currentZPos;

                //
                // Starts moving truck towards user if distance between self and user exceeds treshold in positive direction
                //
                if (zDiff > 0.0f && Mathf.Abs(zDiff) > moveThreshold)
                {
                    StartCoroutine(MoveToZPoint(playerCamera.transform.position.z));
                }
            }
        }
    }

    //
    // Lerps truck and stock from its current Z position to the target Z position
    //
    IEnumerator MoveToZPoint(float targetZ)
    {
        moving = true;

        var initialPos = transform.position;
        var destinationZ = targetZ + localOffset.localPosition.z;
        destinationZ = Mathf.Clamp(destinationZ, minZ + localOffset.localPosition.z, maxZ + localOffset.localPosition.z);
        var targetPos = new Vector3(initialPos.x, initialPos.y, destinationZ);
        
        var range = targetPos.z - initialPos.z;
        var totDeltaZ = 0.0f;

        while (initialPos.z + totDeltaZ <= targetPos.z)
        {
            var currentZ = initialPos.z + totDeltaZ;
            var percent = (currentZ - initialPos.z) / range;
            var newPos = Vector3.Lerp(initialPos, targetPos, Mathf.SmoothStep(0, 1, percent));
            var posChange = newPos - transform.position;

            transform.position = newPos;
            MoveStock(posChange);

            yield return null;

            totDeltaZ += moveSpeed * Time.deltaTime;
        }

        transform.position = targetPos;

        moving = false;
    }

    //
    // Translates all stock currently carried by truck by given amount
    //
    private void MoveStock(Vector3 posChange)
    {
        foreach (var stock in carryingArea.CarriedStock)
        {
            stock.transform.position += posChange;
        }
    }
}
