using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;
using OculusSampleFramework;

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
    [SerializeField] private OVRCameraRig playerRig;
    [SerializeField] private GameObject playerHead;

    [Header("Controller")]
    [SerializeField] private float grabBegin = 0.55f;
    [SerializeField] private float grabEnd = 0.35f;

    [HideInInspector]
    public Direction CurrentDirection { get; private set; }
    [HideInInspector]
    public OVRInput.Axis1D GripButton { get; set; } = OVRInput.Axis1D.PrimaryHandTrigger;

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

    private void Start()
    {
        GripButton = FindObjectOfType<Settings>()?.CurrentGripButton ?? GripButton;
    }

    void Update()
    {
        CalculateRequestedDirection();

        float prevFlex = currFlex;
        currFlex = OVRInput.Get(GripButton, _hand.Controller);

        var tryGrab = currFlex >= grabBegin && prevFlex < grabBegin;
        var tryDrop = currFlex <= grabEnd && prevFlex > grabEnd;

        if (currFlex > grabBegin)
            IsFlexed = true;
        else
            IsFlexed = false;

        if (grabbedStock == null && focusedStock != null && tryGrab)
            GrabBegin();

        if (grabbedStock != null && tryDrop)
            GrabEnd();


#if (UNITY_EDITOR)
        if (CurrentDirection == Direction.Right && previousDirection != Direction.Right && currFlex <= grabEnd)
        {
            playerRig.gameObject.transform.RotateAround(playerHead.transform.position, Vector3.up, 45);
        }
        else if (CurrentDirection == Direction.Left && previousDirection != Direction.Left && currFlex <= grabEnd)
        {
            playerRig.gameObject.transform.RotateAround(playerHead.transform.position, Vector3.up, -45);
        }
#endif
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
