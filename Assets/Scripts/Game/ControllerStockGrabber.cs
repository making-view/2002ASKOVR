﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class ControllerStockGrabber : StockGrabber
{
    [SerializeField] private GameObject playerCamera;

    [Header("Controller")]
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    public Direction CurrentDirection { get; private set; }
    private Direction previousDirection;

    private float dirThreshold = 0.85f;
    private float currFlex = 0.0f;

    private Hand _hand;
    public OVRHand.Hand HandType
    {
        get
        {
            if (_hand == null)
            {
                _hand = GetComponent<Hand>();
            }

            return _hand.Controller == OVRInput.Controller.LTouch ? OVRHand.Hand.HandLeft : OVRHand.Hand.HandRight;
        }
    }

    void Update()
    {
        CalculateRequestedDirection();

        float prevFlex = currFlex;
        currFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _hand.Controller);

        var tryGrab = currFlex >= grabBegin && prevFlex < grabBegin;
        var tryDrop = currFlex <= grabEnd && prevFlex > grabEnd;

        if (grabbedStock == null && focusedStock != null && tryGrab)
            GrabBegin();

        if (grabbedStock != null && tryDrop)
            GrabEnd();

        if (CurrentDirection == Direction.Right && previousDirection != Direction.Right && currFlex <= grabEnd)
        {
            playerCamera.gameObject.transform.Rotate(Vector3.up, 45, Space.World);
        }
        else if (CurrentDirection == Direction.Left && previousDirection != Direction.Left && currFlex <= grabEnd)
        {
            playerCamera.gameObject.transform.Rotate(Vector3.up, -45, Space.World);
        }
    }

    private void CalculateRequestedDirection()
    {
        var newDir = Direction.None;
        var stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _hand.Controller);

        if (stick.x > dirThreshold)
            newDir = Direction.Right;
        else if (stick.x < -dirThreshold)
            newDir = Direction.Left;
        else if (stick.y > dirThreshold)
            newDir = Direction.Up;
        else if (stick.y < -dirThreshold)
            newDir = Direction.Down;

        previousDirection = CurrentDirection;
        CurrentDirection = newDir;
    }
}
