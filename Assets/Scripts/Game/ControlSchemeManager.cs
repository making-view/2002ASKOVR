using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSchemeManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> handObjects;
    [SerializeField] public List<GameObject> controllerObjects;

    public bool IsHandTracking
    {
        get
        {
            return currFrameHandTrackingEnabled;
        }
    }

    bool prevFrameHandTrackingEnabled = true;
    bool currFrameHandTrackingEnabled = true;

    private void Update()
    {
        currFrameHandTrackingEnabled = OVRPlugin.GetHandTrackingEnabled();

        if (currFrameHandTrackingEnabled != prevFrameHandTrackingEnabled)
        {
            if (currFrameHandTrackingEnabled)
            {
                foreach (var obj in handObjects)
                    obj.SetActive(true);

                foreach (var obj in controllerObjects)
                    obj.SetActive(false);
            }
            else
            {
                foreach (var obj in handObjects)
                    obj.SetActive(false);

                foreach (var obj in controllerObjects)
                    obj.SetActive(true);
            }
        }

        prevFrameHandTrackingEnabled = currFrameHandTrackingEnabled;
    }
}
