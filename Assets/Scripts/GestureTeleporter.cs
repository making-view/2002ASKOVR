using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSkeleton))]
public class GestureTeleporter : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GestureDetector gestureDetector = null;
    [SerializeField] private OVRCameraRig cameraRig = null;
    [SerializeField] private GameObject targetMarkerPrefab = null;

    [Header("Settings")]
    public float rayLength = 10f;
    public float reqGestureChangeSpeed = 0.4f;

    private GameObject targetMarker = null;
    private Vector3 targetMarkerInitScale;

    private float teleportActivationTimer = 0;
    private float targetMarkerScaleFactor = 10;

    public OVRSkeleton Skeleton { get; private set; }
    public bool IsTargetMarkerActive
    {
        get
        {
            return targetMarker.activeSelf;
        }
    }

    public void Initialize()
    {
        Skeleton = GetComponent<OVRSkeleton>();
        targetMarker = Instantiate(targetMarkerPrefab);
        targetMarker.SetActive(false);
        targetMarkerInitScale = targetMarker.transform.localScale;
    }

    void Update()
    {
        teleportActivationTimer -= Time.deltaTime;

        if (teleportActivationTimer <= 0.0f)
        {
            targetMarker.SetActive(false);
        }

        //
        // Stops loop execution if hand is not visible to prevent accidental activation while hand is not tracking
        //
        if (!Skeleton.gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
        {
            return;
        }

        //
        // Detects if the aiming direction intersects with the floor
        //
        if (gestureDetector.IsGestureActive(PoseName.JazzHand))
        {
            //
            // Calculates the start and end point of the aiming ray
            // Both points contain a small upward adjustment to match where it intuitively feels like one is aiming
            //
            Vector3 fingerDirection = Skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.right : transform.right;
            Vector3 forwardDirection = Skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.up : transform.up;
            Vector3 start = transform.position + fingerDirection * 0.08f;
            Vector3 end = start + (forwardDirection * rayLength) + (fingerDirection * (rayLength / 2)); 

            Ray ray = new Ray(start, (end - start).normalized);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, rayLength))
            {
                //
                // Reset timer, activate target marker and move marker to where the ray hit the floor
                // Also scales and rotates target marker relative to camera position
                //
                if (rayHit.collider.gameObject.tag == "Floor")
                {
                    teleportActivationTimer = reqGestureChangeSpeed;

                    Vector3 direction = rayHit.point - targetMarker.transform.position;
                    targetMarker.transform.position += direction * Mathf.Clamp(direction.magnitude, 0.0f, 1.0f);

                    Vector3 cameraDistance = targetMarker.transform.position - cameraRig.transform.position;
                    Vector3 newScale = targetMarkerInitScale + (Vector3.one * cameraDistance.magnitude / targetMarkerScaleFactor);
                    Vector3 newForward = new Vector3(cameraDistance.z, 0, -cameraDistance.x).normalized;
                    targetMarker.transform.localScale = newScale;
                    targetMarker.transform.forward = newForward;

                    targetMarker.SetActive(true);
                }
            }
        }

        //
        // Move player to target location
        //
        if (gestureDetector.IsGestureActive(PoseName.Fist) && teleportActivationTimer > 0.0f)
        {
            cameraRig.transform.position = targetMarker.transform.position;
            teleportActivationTimer = 0.0f;
        }
    }
}
