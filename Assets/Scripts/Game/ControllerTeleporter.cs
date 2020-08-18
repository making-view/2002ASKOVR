using OVRTouchSample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class ControllerTeleporter : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.None;
    [SerializeField] private OVRCameraRig cameraRig = null;
    [SerializeField] private GameObject targetMarkerPrefab = null;

    [Header("Settings")]
    public float rayLength = 10f;
    public float buttonReleaseBuffer = 0.4f;

    private Hand hand = null;
    private GameObject targetMarker = null;
    private Camera centerEyeAnchor = null;
    private Vector3 targetMarkerInitScale;
    private Color targetMarkerInitColor;
    private Material targetMarkerMaterial;

    private float teleportActivationTimer = 0;
    private float targetMarkerScaleFactor = 10;
    private int rayLayerMask = 0;

    public bool IsTargetMarkerActive
    {
        get
        {
            return targetMarker.activeSelf;
        }
    }

    public void Start()
    {
        centerEyeAnchor = cameraRig.GetComponentsInChildren<Camera>().ToList().FirstOrDefault(c => c.name == "CenterEyeAnchor");
        hand = GetComponent<Hand>();

        targetMarker = Instantiate(targetMarkerPrefab);
        targetMarkerMaterial = targetMarker.GetComponent<MeshRenderer>().material;
        targetMarkerInitScale = targetMarker.transform.localScale;
        targetMarkerInitColor = targetMarkerMaterial.GetColor("_EmissionColor");

        int collidableLayerMask = 1 << 8;
        int stockLayerMask = 1 << 9;
        int teleportBlockerLayerMask = 1 << 11;
        rayLayerMask = collidableLayerMask | stockLayerMask | teleportBlockerLayerMask;

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
        // Sets marker color to white, will remain this color unless user aims at a teleportable location on the floor
        //
        if (targetMarker.activeSelf)
        {
            targetMarkerMaterial.SetColor("_EmissionColor", Color.white);
        }

        //
        // Detects if the aiming direction intersects with the floor
        //
        if (OVRInput.Get(OVRInput.Button.One, controller))
        {
            //
            // Calculates the start and end point of the aiming ray
            // Magic numbers are eyeball-adjustments to make ray point more closer to where aim feels like it should point
            //
            Vector3 start = hand.PointerPose.position;
            Vector3 end = start + hand.PointerPose.forward * rayLength;

            RaycastHit rayHit;

            if (Physics.Linecast(start, end, out rayHit, rayLayerMask))
            {
                if (rayHit.collider.gameObject.tag == "Floor")
                {
                    teleportActivationTimer = buttonReleaseBuffer;

                    if (targetMarker.activeSelf)
                    {
                        //
                        // Nudges marker partially or completely toward new ray hit point depending on distance from camera
                        // and distance from previous marker location. This smooths out the jittery nature of hand-tracking, no wait, controller-tracking
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
        if (!OVRInput.Get(OVRInput.Button.One, controller) && teleportActivationTimer > 0.0f)
        {
            Vector3 userLocalPos = centerEyeAnchor.transform.localPosition;
            Vector3 xzPlaneOffset = new Vector3(userLocalPos.x, 0, userLocalPos.z);

            cameraRig.transform.position = targetMarker.transform.position - xzPlaneOffset;

            teleportActivationTimer = 0.0f;
        }
    }
}
