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
    [SerializeField] private float rotationTime = 0.25f;
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

            if (!isRotating && !needsReset && currDirection != Direction.None)
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

    //
    // Adjusts the stock angle variables based on given direction
    //
    private void RotateInDirection(Direction direction)
    {
        var handleForward = new Vector3(-grabHandle.lastSpringCoil.transform.up.x, 0, -grabHandle.lastSpringCoil.transform.up.z);
        var handleYaw = Vector3.SignedAngle(Vector3.forward, handleForward, Vector3.up);
        var isClosestToZAxis = (handleYaw < 45 && handleYaw > -45) || (handleYaw > 135 && handleYaw < -135);

        var newX = stockXAngle;
        var newY = stockYAngle;
        var newZ = stockZAngle;

        switch (direction)
        {
            case Direction.Left:
                newY = WrapAngle(stockYAngle - 90);
                break;
            case Direction.Right:
                newY = WrapAngle(stockYAngle + 90);
                break;
            case Direction.Up:
                SetForwardRotation(isClosestToZAxis, out newX, out newZ, reverse: false);
                break;
            case Direction.Down:
                SetForwardRotation(isClosestToZAxis, out newX, out newZ, reverse: true);
                break;
        }

        StartCoroutine(RotateStock(newX, newY, newZ));
    }

    //
    // Rotates forwards along Z or X axis, or backwards if reverse is true
    //
    private void SetForwardRotation(bool isClosestToZAxis, out int newX, out int newZ, bool reverse)
    {
        var r = reverse ? -1 : 1;
        newX = stockXAngle;
        newZ = stockZAngle;

        if (isClosestToZAxis)
        {
            if (stockYAngle == 0)
                newX = WrapAngle(stockXAngle + (90 * r));
            else if (stockYAngle == 90)
                newZ = WrapAngle(stockZAngle + (90 * r));
            else if (stockYAngle == 180)
                newX = WrapAngle(stockXAngle - (90 * r));
            else if (stockYAngle == 270)
                newZ = WrapAngle(stockZAngle - (90 * r));
        }
        else
        {
            if (stockYAngle == 0)
                newZ = WrapAngle(stockZAngle - (90 * r));
            else if (stockYAngle == 90)
                newX = WrapAngle(stockXAngle + (90 * r));
            else if (stockYAngle == 180)
                newZ = WrapAngle(stockZAngle + (90 * r));
            else if (stockYAngle == 270)
                newX = WrapAngle(stockXAngle - (90 * r));
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
    // Lerps Stock towards new rotation
    //
    private IEnumerator RotateStock(int newX, int newY, int newZ)
    {
        isRotating = true;

        var currRotation = transform.rotation;
        var newRotation = Quaternion.Euler(newX, newY, newZ);
        var timer = 0.0f;

        while (timer <= rotationTime)
        {
            var percent = timer / rotationTime;

            transform.rotation = Quaternion.Lerp(currRotation, newRotation, Mathf.SmoothStep(0, 1, percent));

            yield return null;

            timer += Time.deltaTime;
        }

        stockXAngle = newX;
        stockYAngle = newY;
        stockZAngle = newZ;

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
