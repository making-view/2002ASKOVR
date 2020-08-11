using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSchemeManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> handObjects;
    [SerializeField] public List<GameObject> controllerObjects;

    bool prevFrameHandTrackingEnabled = true;

    private void Update()
    {
        var handTrackingEnabled = OVRPlugin.GetHandTrackingEnabled();

        if (handTrackingEnabled != prevFrameHandTrackingEnabled)
        {
            if (handTrackingEnabled)
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

        prevFrameHandTrackingEnabled = handTrackingEnabled;
    }
}
