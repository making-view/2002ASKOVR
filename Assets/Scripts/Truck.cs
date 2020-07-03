using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private GameObject destinationPoint = null;
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private float moveThreshold = 1.5f;
    [SerializeField] private float moveTime = 3f;

    private bool moving = false;

    // Update is called once per frame
    void Update()
    {
        if (!moving && (transform.position.z - playerCamera.transform.position.z) > moveThreshold)
        {
            StartCoroutine(MoveToPoint(playerCamera.transform.position.z));
        }
    }

    IEnumerator MoveToPoint(float targetZ)
    {
        moving = true;

        var timer = 0.0f;
        var initalPos = transform.position;
        var destinationZ = targetZ - destinationPoint.transform.localPosition.z;
        var targetPos = new Vector3(initalPos.x, initalPos.y, targetZ);

        while (timer <= moveTime)
        {
            var percent = timer / moveTime;
            var newPos = Vector3.Lerp(initalPos, targetPos, Mathf.SmoothStep(0, 1, percent));

            transform.position = newPos;

            yield return null;

            timer += Time.deltaTime;
        }

        transform.position = targetPos;

        moving = false;
    }
}
