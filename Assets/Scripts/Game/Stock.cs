using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OculusSampleFramework;

[RequireComponent(typeof(ButtonController))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Stock : MonoBehaviour
{
    // How long the object has to be stationary before its physics are turned off
    [SerializeField] private float stationaryTime = 1.0f;
    [SerializeField] private Text massDisplay = null;
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
            var newPos = new Vector3();
            var newRot = new Quaternion();

            // 
            // Hands and controllers handle rotation and movement differently
            //
            if (controlScheme.IsHandTracking)
                HandTrackingMovement(out newPos, out newRot);
            else
                ControllerMovement(out newPos, out newRot);


            //
            // Ensures stock items do not penetrate each other
            //
            Vector3 direction = Vector3.zero;
            float distance = 0.0f;
            foreach (var collider in otherColliders)
            {
                if (Physics.ComputePenetration(ownCollider, newPos, newRot, collider,
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
            transform.rotation = newRot;
        }
    }
    
    //
    // Moves the stock along with GrabHandle and executes gesture controls
    //
    private void HandTrackingMovement(out Vector3 newPos, out Quaternion newRot)
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

        var lastCoilTrans = grabHandle.lastSpringCoil.transform;
        newRot = Quaternion.Euler(new Vector3(stockXAngle, stockYAngle, stockZAngle));
        newPos = lastCoilTrans.position - (lastCoilTrans.up * floatDistance);

        previousHandleRoll = handleRoll;
        previousHandleYaw = handleYaw;
    }

    //
    // Moves the stock along with GrabHandle and executes controller button controls
    //
    private void ControllerMovement(out Vector3 newPos, out Quaternion newRot)
    {
        var grabber = grabbedBy as ControllerStockGrabber;

        if (grabber)
        {
            var currDirection = grabber.CurrentDirection;

            if (currDirection != previousDirection)
            {
                needsReset = false;
            }

            if (!needsReset && currDirection != Direction.None)
            {
                RotateInDirection(currDirection);

                needsReset = true;
            }

            previousDirection = currDirection;
        }

        var lastCoilTrans = grabHandle.lastSpringCoil.transform;
        newRot = Quaternion.Euler(new Vector3(stockXAngle, stockYAngle, stockZAngle));
        newPos = lastCoilTrans.position - (lastCoilTrans.up * floatDistance);
    }

    private void RotateInDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                stockYAngle = WrapAngle(stockYAngle - 90);
                break;
            case Direction.Right:
                stockYAngle = WrapAngle(stockYAngle + 90);
                break;
            case Direction.Up:
                break;
            case Direction.Down:
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

    private int WrapAngle(int angle)
    {
        if (angle >= 360)
            angle %= 360;
        else if (angle < 0)
            angle += 360;

        return angle;
    }
}
