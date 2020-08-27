using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OculusSampleFramework;

public enum LoadCarryingSides
{
    All,
    TopAndBottom,
    None
}

[RequireComponent(typeof(ButtonController))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Stock : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private string itemName = "Vare";
    [SerializeField] private LoadCarryingSides loadCarryingSides = LoadCarryingSides.All;

    [Header("Config")]
    [SerializeField] private Text massDisplay = null;

    [Header("Settings")] 
    [Tooltip("How long the object has to be stationary before its physics are turned off")]
    [SerializeField] private float stationaryTime = 1.0f;
    [SerializeField] private float rotationTime = 0.25f;

    public string StockCode { get; set; }

    private float angleAdjustCooldownTime = 0.75f;
    private float angleAdjustVelocity = 500;

    private BoxCollider ownCollider;
    private Rigidbody rigidBody;
    private List<BoxCollider> otherColliders;
    private ControlSchemeManager controlScheme;

    private StockGrabber grabbedBy = null;
    private GrabHandle grabHandle = null;
    private float floatDistance = 0.0f;
    private float movementSensitivity = 0.01f;
    private bool isRotating = false;

    //
    // Angles
    //
    private int stockXAngle = 0;
    private int stockYAngle = 0;
    private int stockZAngle = 0;
    
    //
    // Hands rotation
    //
    private float angleAdjustThreshold = 30.0f;
    private float angleAdjustCooldownTimer = 0.0f;
    private float previousHandleRoll = 0.0f;
    private float previousHandleYaw = 0.0f;

    //
    // Controller rotation
    //
    private bool needsReset = false;
    private Direction previousDirection = Direction.None;

    void Start()
    {
        ownCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
        controlScheme = FindObjectOfType<ControlSchemeManager>();

        if (massDisplay != null)
        {
            massDisplay.text = rigidBody.mass + " KG";
        }

        otherColliders = new List<BoxCollider>();
    }

    void Update()
    {
        angleAdjustCooldownTimer -= Time.deltaTime;

        //
        // If attached to a grabHandle, move along with it
        //
        if (grabHandle)
        {
            // 
            // Hands and controllers handle rotation differently
            //
            if (controlScheme.IsHandTracking)
                HandTrackingRotation();
            else
                ControllerRotation();

            var lastCoilTrans = grabHandle.lastSpringCoil.transform;
            var newPos = lastCoilTrans.position - (lastCoilTrans.up * floatDistance);

            //
            // Ensures stock items do not penetrate each other
            //
            Vector3 direction = Vector3.zero;
            float distance = 0.0f;
            foreach (var collider in otherColliders)
            {
                if (Physics.ComputePenetration(ownCollider, newPos, transform.rotation, collider,
                    collider.gameObject.transform.position, collider.gameObject.transform.rotation,
                    out direction, out distance))
                {
                    newPos += direction * distance;
                }
            }

            //
            // Applies the calculated rotation and the new and corrected position
            //
            transform.position = newPos;
        }
    }
    
    //
    // Moves the stock along with GrabHandle and executes gesture controls
    //
    private void HandTrackingRotation()
    {
        var handleForward = new Vector3(grabHandle.transform.forward.x, 0, grabHandle.transform.forward.z);
        var handleRoll = Vector3.SignedAngle(Vector3.up, grabHandle.transform.right, handleForward);
        var handleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);

        var forwardUpAngle = Vector3.Angle(Vector3.up, grabHandle.transform.forward);
        var canAdjustAngles = angleAdjustCooldownTimer <= 0.0f
            && 90 - angleAdjustThreshold <= forwardUpAngle
            && 90 + angleAdjustThreshold >= forwardUpAngle;

        //
        // Only allows rotation if handle is held pointing approximately downwards
        //
        if (canAdjustAngles)
        {
            var smallestRollChange = handleRoll - previousHandleRoll;
            smallestRollChange += smallestRollChange > 180 ? -360 : (smallestRollChange < -180) ? 360 : 0;
            var smallestYawChange = handleYaw - previousHandleYaw;
            smallestYawChange += smallestYawChange > 180 ? -360 : (smallestYawChange < -180) ? 360 : 0;

            var handleRollVelocity = smallestRollChange / Time.deltaTime;
            var handleYawVelocity = smallestYawChange / Time.deltaTime;

            if (Mathf.Abs(handleYawVelocity) >= angleAdjustVelocity)
            {
                stockYAngle += (int)Mathf.Sign(handleYawVelocity) * 90;
                stockYAngle = WrapAngle(stockYAngle);
                angleAdjustCooldownTimer = angleAdjustCooldownTime;
            }
            else if (Mathf.Abs(handleRollVelocity) >= angleAdjustVelocity)
            {
                var flipRotationAxes = stockYAngle == 90 || stockYAngle == 270;
                var isClosestToZAxis = (handleYaw < 45 && handleYaw > -45) || (handleYaw > 135 && handleYaw < -135);
                var shouldRotateAroundZ = (isClosestToZAxis && !flipRotationAxes) || (!isClosestToZAxis && flipRotationAxes);

                if (shouldRotateAroundZ)
                {
                    stockZAngle += (int)Mathf.Sign(handleRollVelocity) * 90;
                    stockZAngle = WrapAngle(stockZAngle);
                }
                else
                {
                    stockXAngle += (int)Mathf.Sign(handleRollVelocity) * 90;
                    stockXAngle = WrapAngle(stockXAngle);
                }

                angleAdjustCooldownTimer = angleAdjustCooldownTime;
            }
        }

        previousHandleRoll = handleRoll;
        previousHandleYaw = handleYaw;

        var newRot = Quaternion.Euler(new Vector3(stockXAngle, stockYAngle, stockZAngle));
        transform.rotation = newRot;
    }

    //
    // Moves the stock along with GrabHandle and executes controller button controls
    //
    private void ControllerRotation()
    {
        var grabber = grabbedBy as ControllerStockGrabber;

        if (grabber)
        {
            var currDirection = grabber.CurrentDirection;

            if (currDirection != previousDirection)
            {
                needsReset = false;
            }

            if (!isRotating && !needsReset && currDirection != Direction.None)
            {
                RotateInDirection(currDirection);

                needsReset = true;
            }

            previousDirection = currDirection;
        }
    }

    //
    // Adjusts the stock angle variables based on given direction
    //
    private void RotateInDirection(Direction direction)
    {
        var handleForward = new Vector3(-grabHandle.lastSpringCoil.transform.up.x, 0, -grabHandle.lastSpringCoil.transform.up.z);
        var handleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);
        var closestRightAngle = WrapAngle(Mathf.RoundToInt(handleYaw / 90) * 90);

        switch (direction)
        {
            case Direction.Left:
                StartCoroutine(RotateStockAround(transform.position, Vector3.up, reverse: true));
                break;
            case Direction.Right:
                StartCoroutine(RotateStockAround(transform.position, Vector3.up, reverse: false));
                break;
            case Direction.Up:
                if (closestRightAngle == 0)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.right, reverse: false));
                else if (closestRightAngle == 90)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.forward, reverse: true));
                else if (closestRightAngle == 180)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.right, reverse: true));
                else if (closestRightAngle == 270)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.forward, reverse: false));
                break;
            case Direction.Down:
                if (closestRightAngle == 0)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.right, reverse: true));
                else if (closestRightAngle == 90)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.forward, reverse: false));
                else if (closestRightAngle == 180)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.right, reverse: false));
                else if (closestRightAngle == 270)
                    StartCoroutine(RotateStockAround(transform.position, Vector3.forward, reverse: true));
                break;
        }
    }

    //
    // Set up attachment of self to the handle of the StockGrabber grabbing this object
    //
    public void Grab(StockGrabber grabber, GrabHandle handle, float fDist, int xAngle, int yAngle, int zAngle)
    {
        rigidBody.isKinematic = true;

        grabbedBy = grabber;
        grabHandle = handle;
        floatDistance = fDist;
        stockXAngle = xAngle;
        stockYAngle = yAngle;
        stockZAngle = zAngle;

        var handleForward = new Vector3(grabHandle.transform.forward.x, 0, grabHandle.transform.forward.z);
        previousHandleRoll = Vector3.SignedAngle(Vector3.up, handle.transform.right, handleForward);
        previousHandleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);
    }

    //
    // Remove reference to grabPoint, enable physics and start coroutine to eventually stop physics again
    //
    public void Drop()
    {
        rigidBody.isKinematic = false;

        grabbedBy = null;
        grabHandle = null;
        StartCoroutine(DisableGravityOnceStationary());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabHandle && collision.collider is BoxCollider)
        {
            otherColliders.Add(collision.collider as BoxCollider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider is BoxCollider && otherColliders.Contains(collision.collider as BoxCollider))
        {
            otherColliders.Remove(collision.collider as BoxCollider);
        }
    }

    //
    // Turns off physics after object has been stationary for some time
    //
    private IEnumerator DisableGravityOnceStationary()
    {
        var timer = 0.0f;
        var previousPosition = Vector3.zero;

        while (timer < stationaryTime)
        {
            timer += Time.deltaTime;

            var diff = transform.position - previousPosition;

            if (diff.magnitude > movementSensitivity) 
                timer = 0.0f;

            previousPosition = transform.position;

            yield return null;
        }

        rigidBody.isKinematic = true;
    }

    //
    // Lerps Stock towards new rotation around given point and axis
    //
    private IEnumerator RotateStockAround(Vector3 point, Vector3 axis, bool reverse)
    {
        isRotating = true;

        var r = reverse ? -1 : 1;
        var timer = 0.0f;
        var targetAngle = 90f * r;
        var prevAngle = 0f;

        while (timer < rotationTime)
        {
            var percent = timer / rotationTime;
            var currAngle = Mathf.SmoothStep(0f, targetAngle, percent);
            var deltaAngle = currAngle - prevAngle;

            transform.RotateAround(point, axis, deltaAngle);

            prevAngle = currAngle;

            yield return null;

            timer += Time.deltaTime;
        }

        var lastAngle = Mathf.SmoothStep(0f, targetAngle, 1);
        var lastDeltaAngle = lastAngle - prevAngle;

        transform.RotateAround(point, axis, lastDeltaAngle);

        isRotating = false;
    }

    private int WrapAngle(int angle)
    {
        if (angle >= 360)
            angle %= 360;
        else if (angle < 0)
            angle += 360;

        return angle;
    }
}
