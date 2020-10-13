using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public OVRInput.Axis1D CurrentGripButton = OVRInput.Axis1D.PrimaryHandTrigger;
    public OVRInput.Axis1D OtherGripButton = OVRInput.Axis1D.PrimaryIndexTrigger;

    public bool RightHanded { get; set; } = true;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Settings level load");
        EnableOrDisableHandScripts();
    }

    public void SwapGripControls(Text text)
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

    public void SwapHandedness(Text text)
    {
        RightHanded = !RightHanded;

        EnableOrDisableHandScripts();

        var newText = RightHanded ? "Høyrehendt" : "Venstrehendt";
        text.text = newText;
    }

    private void EnableOrDisableHandScripts()
    {
        var leftHand = GameObject.Find("LeftHandAnchor");
        var rightHand = GameObject.Find("RightHandAnchor");

        var leftKeypad = leftHand.GetComponentInChildren<ActivateKeypad>(true);
        var leftTeleporter = leftHand.GetComponentInChildren<ControllerTeleporter>(true);
        var rightKeypad = rightHand.GetComponentInChildren<ActivateKeypad>(true);
        var rightTeleporter = rightHand.GetComponentInChildren<ControllerTeleporter>(true);

        rightTeleporter.enabled = RightHanded;
        leftKeypad.enabled = RightHanded;

        leftTeleporter.enabled = !RightHanded;
        rightKeypad.enabled = !RightHanded;
    }
}
