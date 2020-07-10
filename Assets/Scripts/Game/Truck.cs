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
    [SerializeField] private float moveTime = 3f;

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

            //
            // Places truck and its stock in center of new lane
            //
            if (closestLane != currentLane)
            {
                currentLane = closestLane;

                var newLane = truckLanes.GetLanePosition(currentLane);
                var newPosition = new Vector3(newLane.x, transform.position.y, 0);
                var posChange = newPosition - transform.position;

                transform.position = newPosition;
                MoveStock(posChange);
            }

            var currentZPos = transform.position.z + localOffset.localPosition.z;
            var zDiff = Mathf.Abs(currentZPos - playerCamera.transform.position.z);

            //
            // Starts moving truck towards user if distance between self and user exceeds treshold
            //
            if (zDiff > moveThreshold)
            {
                StartCoroutine(MoveToZPoint(playerCamera.transform.position.z));
            }
        }
    }

    //
    // Lerps truck and stock from its current Z position to the target Z position
    //
    IEnumerator MoveToZPoint(float targetZ)
    {
        moving = true;

        var timer = 0.0f;
        var initalPos = transform.position;
        var destinationZ = targetZ - localOffset.localPosition.z;
        var targetPos = new Vector3(initalPos.x, initalPos.y, destinationZ);

        while (timer <= moveTime)
        {
            var percent = timer / moveTime;
            var newPos = Vector3.Lerp(initalPos, targetPos, Mathf.SmoothStep(0, 1, percent));
            var posChange = newPos - transform.position;

            transform.position = newPos;
            MoveStock(posChange);

            yield return null;

            timer += Time.deltaTime;
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
