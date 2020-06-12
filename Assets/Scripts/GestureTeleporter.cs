using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSkeleton))]
public class GestureTeleporter : MonoBehaviour
{
    public GestureDetector gestureDetector;
    public OVRCameraRig cameraRig;
    public GameObject targetMarkerPrefab;
    public float rayLength = 10f;
    public float reqGestureChangeSpeed = 0.4f;

    private OVRSkeleton skeleton;
    private GameObject targetMarker;

    private float teleportActivationTimer = 0;

    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();
        targetMarker = Instantiate(targetMarkerPrefab);
        targetMarker.SetActive(false);
    }

    void Update()
    {
        teleportActivationTimer -= Time.deltaTime;

        if (teleportActivationTimer <= 0.0f)
        {
            targetMarker.SetActive(false);
        }

        if (gestureDetector.IsGestureActive(PoseName.JazzHand))
        {
            Vector3 palmUp = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.right : transform.right;
            Vector3 rayPointDirection = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.up : transform.up;
            Vector3 start = transform.position + palmUp * 0.08f;
            Vector3 end = start + (rayPointDirection * rayLength) + (palmUp * (rayLength / 2));

            Ray ray = new Ray(start, (end - start).normalized);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, rayLength))
            {
                Debug.Log("Ray hit: " + rayHit.collider.gameObject.name);

                if (rayHit.collider.gameObject.tag == "Floor")
                {
                    teleportActivationTimer = reqGestureChangeSpeed;

                    Vector3 direction = rayHit.point - targetMarker.transform.position;
                    targetMarker.transform.position += direction * Mathf.Clamp(direction.magnitude, 0.0f, 1.0f);

                    targetMarker.SetActive(true);
                }
            }
        }

        if (gestureDetector.IsGestureActive(PoseName.Fist) && teleportActivationTimer > 0.0f)
        {
            cameraRig.transform.position = targetMarker.transform.position;
            teleportActivationTimer = 0.0f;
        }
    }
}
