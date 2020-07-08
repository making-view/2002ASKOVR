using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Camera centerEyeAnchor = null;
    private Vector3 targetMarkerInitScale;
    private Color targetMarkerInitColor;
    private Material targetMarkerMaterial;

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
        centerEyeAnchor = cameraRig.GetComponentsInChildren<Camera>().ToList().FirstOrDefault(c => c.name == "CenterEyeAnchor");

        targetMarker = Instantiate(targetMarkerPrefab);
        targetMarkerMaterial = targetMarker.GetComponent<MeshRenderer>().material;
        targetMarkerInitScale = targetMarker.transform.localScale;
        targetMarkerInitColor = targetMarkerMaterial.GetColor("_EmissionColor");

        targetMarker.SetActive(false);
    }

    void Update()
    {
        teleportActivationTimer -= Time.deltaTime;

        if (teleportActivationTimer <= 0.0f)
        {
            targetMarker.SetActive(false);
        }

        //
        // Sets marker color to white, will remain this color until user aims at a teleportable location on the floor
        //
        if (targetMarker.activeSelf)
        {
            targetMarkerMaterial.SetColor("_EmissionColor", Color.white);
        }

        //
        // Stops loop execution if hand is not visible to prevent accidental activation while hand is not tracking
        //
        if (!Skeleton.IsMeshVisible)
        {
            teleportActivationTimer = 0.0f;
            targetMarker.SetActive(false);
            return;
        }

        //
        // Detects if the aiming direction intersects with the floor
        //
        if (gestureDetector.IsGestureActive(PoseName.OK))
        {
            //
            // Calculates the start and end point of the aiming ray
            // Magic numbers are eyeball-adjustments to make ray point more closer to where aim feels like it should point
            //
            Vector3 handRightDirection = Skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? transform.forward : -transform.forward;
            Vector3 fingerDirection = Skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.right : transform.right;
            Vector3 palmForwardDirection = Skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight ? -transform.up : transform.up;
            Vector3 start = transform.position + (fingerDirection * 0.08f) + (palmForwardDirection * 0.045f);
            Vector3 end = start + (handRightDirection * rayLength) - (fingerDirection * rayLength * 0.25f);

            RaycastHit rayHit;

            if (Physics.Linecast(start, end, out rayHit))
            {
                if (rayHit.collider.gameObject.tag == "Floor")
                {
                    teleportActivationTimer = reqGestureChangeSpeed;

                    if (targetMarker.activeSelf)
                    {
                        //
                        // Nudges marker partially or completely toward new ray hit point depending on distance from camera
                        // and distance from previous marker location. This smooths out the jittery nature of hand-tracking
                        //
                        Vector3 direction = rayHit.point - targetMarker.transform.position;
                        Vector3 distance = rayHit.point - centerEyeAnchor.transform.position;
                        targetMarker.transform.position += direction * Mathf.Clamp(direction.magnitude / (distance.magnitude * 4.0f), 0.0f, 1.0f);
                    }
                    else
                    {
                        targetMarker.transform.position = rayHit.point;
                    }

                    //
                    // Rotates and scales marker depending on where it is relative to the user, 
                    // also sets it to its active aiming color
                    //
                    Vector3 cameraDistance = targetMarker.transform.position - centerEyeAnchor.transform.position;
                    Vector3 newScale = targetMarkerInitScale + (Vector3.one * cameraDistance.magnitude / targetMarkerScaleFactor);
                    Vector3 newForward = new Vector3(cameraDistance.z, 0, -cameraDistance.x).normalized;
                    targetMarker.transform.localScale = newScale;
                    targetMarker.transform.forward = newForward;
                    targetMarkerMaterial.SetColor("_EmissionColor", targetMarkerInitColor);

                    targetMarker.SetActive(true);
                }
            }
        }

        //
        // Move player to target location
        //
        if (gestureDetector.IsGestureActive(PoseName.Fist) && teleportActivationTimer > 0.0f)
        {
            Vector3 userLocalPos = centerEyeAnchor.transform.localPosition;
            Vector3 xzPlaneOffset = new Vector3(userLocalPos.x, 0, userLocalPos.z);

            cameraRig.transform.position = targetMarker.transform.position - xzPlaneOffset;

            teleportActivationTimer = 0.0f;
        }
    }
}
