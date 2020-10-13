using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public OVRInput.Axis1D CurrentGripButton = OVRInput.Axis1D.PrimaryHandTrigger;
    public OVRInput.Axis1D OtherGripButton = OVRInput.Axis1D.PrimaryIndexTrigger;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SwapControls(Text text)
    {
        var curr = CurrentGripButton;
        CurrentGripButton = OtherGripButton;
        OtherGripButton = curr;

        foreach (var csGrabber in FindObjectsOfType<ControllerStockGrabber>())
        {
            csGrabber.GripButton = CurrentGripButton;
        }

        if (text)
        {
            var newText = CurrentGripButton.Equals(OVRInput.Axis1D.PrimaryHandTrigger) ? "Grep" : "Avtrekker";
            text.text = newText;
        }
    }
}
