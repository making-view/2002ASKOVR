using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using OculusSampleFramework;
using System;

public enum LoadCarryingSides
{
    All,
    TopAndBottom,
    None
}

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
    [SerializeField] private float rotationTime = 0.25f;
    [SerializeField] private float tumbleThreshold = 3.0f;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color wrappedColor = Color.black;
    [SerializeField] private Color damageColor = Color.red;

    public string StockCode { get; set; }
    public int ShelfNumber { get; set; }
    public bool IsWrapped { 
        get
        {
            return rigidBody.isKinematic;
        }
    }

    public bool IsTumbling
    {
        get
        {
            return rigidBody.angularVelocity.magnitude > tumbleThreshold;
        }
    }

    private float angleAdjustCooldownTime = 0.75f;
    private float angleAdjustVelocity = 500;

    private BoxCollider ownCollider;
    private Rigidbody rigidBody;
    private List<BoxCollider> otherColliders;
    private ControlSchemeManager controlScheme;
    private GenericAudioHandler audioHandler;
    private GenericAudioSource audioSource;

    private StockGrabber grabbedBy = null;
    private GrabHandle grabHandle = null;
    private float floatDistance = 0.0f;
    private bool isRotating = false;
    private Vector3 previousPosition;
    private Vector3 grabVelocity;

    private Material material;
    private Color initEmissionColor;

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

    public void SetFocused(bool focused)
    {
        material.SetColor("_EmissionColor", focused ? highlightColor : initEmissionColor);
    }

    void Start()
    {
        ownCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
        audioHandler = GetComponent<GenericAudioHandler>();
        audioSource = GetComponent<GenericAudioSource>();
        controlScheme = FindObjectOfType<ControlSchemeManager>();
        previousPosition = transform.position;
        material = GetComponent<MeshRenderer>().material;
        initEmissionColor = material.GetColor("_EmissionColor");

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
            // Applies the calculated rotation and the new and corrected position, and calculates velocity
            //
            grabVelocity = (newPos - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;
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
        if (grabVelocity.magnitude > 2f)
        {
            rigidBody.velocity = grabVelocity / Mathf.Clamp(rigidBody.mass, 1, 15);
        }

        grabbedBy = null;
        grabHandle = null;
    }

    internal void SetWrapped(bool isBelow)
    {
        if (!IsTumbling)
        {
            rigidBody.isKinematic = isBelow;
            GetComponent<ButtonController>().IsInteractable = !isBelow;
            var newColor = isBelow ? wrappedColor : initEmissionColor;
            material.SetColor("_EmissionColor", newColor);
        }
    }

    public void CaptureState(bool causeDamage)
    {
        if (IsTumbling || causeDamage)
        {
            material.SetColor("_EmissionColor", damageColor);
        }

        rigidBody.isKinematic = true;
        GetComponent<ButtonController>().IsInteractable = false;
        
    }

    public List<Stock> GetStockAbove()
    {
        var result = new List<Stock>();

        var center = ownCollider.transform.position;
        center.y += 0.05f;
        var halfExtents = ownCollider.bounds.size / 2.1f;
        
        result = Physics.OverlapBox(center, halfExtents)
            .Where(o => o != ownCollider 
                && o.gameObject.GetComponent<Stock>() != null 
                && o.gameObject.transform.position.y
                    > transform.position.y)
            .Select(o => o.gameObject.GetComponent<Stock>()).ToList();

        return result;
    }

    public List<Stock> GetStockBelow()
    {
        var result = new List<Stock>();

        var center = ownCollider.transform.position;
        center.y -= 0.05f;
        var halfExtents = ownCollider.bounds.size / 2.1f;

        result = Physics.OverlapBox(center, halfExtents)
            .Where(o => o != ownCollider
                && o.gameObject.GetComponent<Stock>() != null
                && o.gameObject.transform.position.y
                    < transform.position.y)
            .Select(o => o.gameObject.GetComponent<Stock>()).ToList();

        return result;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        var audioMaxStrength = Mathf.Clamp(collisionForce, 500f, 2000f).Map(500f, 2000f, 0f, 1f);
        var audioMinStrength = Mathf.Clamp(audioMaxStrength - 0.1f, 0f, 1f);

        if (audioMaxStrength > 0f && audioHandler && audioSource)
        {
            audioHandler.volumeMin = audioMinStrength;
            audioHandler.volumeMax = audioMaxStrength;
            audioSource.UpdateVariablesFromHandler();
            audioSource.PlaySound();
        }

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
