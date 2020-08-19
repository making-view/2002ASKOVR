using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

public class ControllerStockGrabber : StockGrabber
{
    [Header("Controller")]
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    private float currFlex = 0.0f;

    private Hand _hand;
    public OVRHand.Hand HandType
    {
        get
        {
            return _hand.Controller == OVRInput.Controller.LTouch ? OVRHand.Hand.HandLeft : OVRHand.Hand.HandRight;
        }
    }

    void Start()
    {
        _hand = GetComponent<Hand>();
    }

    void Update()
    {
        float prevFlex = currFlex;
        currFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _hand.Controller);

        var tryGrab = currFlex >= grabBegin && prevFlex < grabBegin;
        var tryDrop = currFlex <= grabEnd && prevFlex > grabEnd;

        if (grabbedStock == null && focusedStock != null && tryGrab)
            GrabBegin();

        if (grabbedStock != null && tryDrop)
            GrabEnd();
    }
}
