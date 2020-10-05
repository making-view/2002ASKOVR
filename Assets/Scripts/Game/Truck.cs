using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    const int CLIP = 0;
    const int LOOP = 1;

    [Header("Config")]
    [SerializeField] private CarryingArea carryingArea = null;
    [SerializeField] private TruckLanes truckLanes = null;
    [SerializeField] private Transform localOffset = null;
    [SerializeField] private Camera playerCamera = null;

    [Header("Audio")]
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip loopSound;
    [SerializeField] AudioClip endSound;

    [Header("Settings")]
    [SerializeField] private float moveThreshold = 1.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float truckEndPointZ = 0.0f;

    public float TotalMovement { get; private set; } = 0.0f;
    public float UnsafeMovement { get; private set; } = 0.0f;
    public bool StockFellOff { get; private set; } = false;

    private bool moving = false;
    AudioSource[] audioSources;

    private void Awake()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
        audioSources[LOOP].clip = loopSound;
    }

    //
    // Determines if truck should across or between lanes, then executes the required movement
    //
    void Update()
    {
        //
        // Do not move truck if it is already moving
        //
        if (!moving)
        {
            var closestLane = truckLanes.FindLaneClosestToPoint(playerCamera.transform.position);

            if (closestLane == 1)
            {
                var currentZPos = transform.position.z - localOffset.localPosition.z;
                var zDiff = playerCamera.transform.position.z - currentZPos;

                //
                // Starts moving truck towards user if distance between self and user exceeds treshold in positive direction
                //
                if (currentZPos < truckEndPointZ && zDiff > 0.0f && Mathf.Abs(zDiff) > moveThreshold)
                {
                    StartCoroutine(MoveToZPoint(playerCamera.transform.position.z));
                }
            }
        }
    }

    //
    // Hides the truck meshes, captures current state of all carried stock, and stops this script from running 
    //
    public void DisableTruck()
    {
        StopAllCoroutines();

        foreach (var stock in carryingArea.CarriedStock)
        {
            stock.CaptureState(false);
            stock.transform.parent = transform;
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponentsInChildren<MeshRenderer>().ToList()
            .FirstOrDefault(c => c.transform.parent.name == "Geometry")
            .transform.parent.gameObject.SetActive(false);

        enabled = false;
    }

    //
    // Lerps truck and stock from its current Z position to the target Z position
    //
    IEnumerator MoveToZPoint(float targetZ)
    {
        moving = true;
        var stopping = false;

        #region Initial state and move target setup
        var initStock = carryingArea.CarriedStock.ToList();
        var isMovementSafe = IsMovementSafe();

        var initialPos = transform.position;
        var destinationZ = targetZ + localOffset.localPosition.z;
        destinationZ = Mathf.Clamp(destinationZ, initialPos.z, truckEndPointZ - localOffset.localPosition.z);
        var targetPos = new Vector3(initialPos.x, initialPos.y, destinationZ);

        var range = targetPos.z - initialPos.z;
        var totDeltaZ = 0.0f;
        #endregion

        #region Audio setup
        StopSounds();

        audioSources[CLIP].clip = startSound;
        audioSources[CLIP].Play();

        var distanceToMove = (targetPos - initialPos).magnitude;
        var moveTime = distanceToMove / moveSpeed;
        var completeSequenceDuration = startSound.length + endSound.length + (loopSound.length / 2);
        var endSoundThreshold = endSound.length + (loopSound.length / 2);

        //
        // Skips the looping part if the drive is short (less time than the start and stop sounds combined)
        //
        bool longDrive;
        if (moveTime > completeSequenceDuration)
        {
            audioSources[LOOP].clip = loopSound;
            audioSources[LOOP].loop = true;
            audioSources[LOOP].PlayDelayed(startSound.length);
            longDrive = true;
        }
        else
        {
            audioSources[LOOP].clip = endSound;
            audioSources[LOOP].loop = false;
            audioSources[LOOP].PlayDelayed(startSound.length);
            longDrive = false;
        }
        #endregion

        #region Movement loop
        while (initialPos.z + totDeltaZ <= targetPos.z)
        {
            var currentZ = initialPos.z + totDeltaZ;
            var percent = (currentZ - initialPos.z) / range;
            var newPos = Vector3.Lerp(initialPos, targetPos, Mathf.SmoothStep(0, 1, percent));
            var posChange = newPos - transform.position;

            transform.position = newPos;
            TotalMovement += posChange.magnitude;
            UnsafeMovement += isMovementSafe ? 0.0f : posChange.magnitude;

            var stockStaying = MoveStock(posChange);

            if (!stockStaying)
            {
                StockFellOff = true;
                StopSounds();
                FindObjectOfType<GameManager>().EndGame();
                targetPos = transform.position;
                break;
            }

            var currentStock = carryingArea.CarriedStock.ToList();
            var initCurrStockIntersection = initStock.Intersect(currentStock).ToList();
            var allInitStockStillOn = initCurrStockIntersection.Count == initStock.Count;

            if (!allInitStockStillOn)
            {
                StockFellOff = true;

                foreach (var stock in initCurrStockIntersection)
                {
                    initStock.Remove(stock);
                }

                foreach (var stock in initStock)
                {
                    stock.CaptureState(true);
                    stock.transform.parent = transform;
                }

                StopSounds();
                FindObjectOfType<GameManager>().EndGame();
                targetPos = transform.position;
                break;
            }

            distanceToMove = (targetPos - transform.position).magnitude;
            moveTime = distanceToMove / moveSpeed;

            if (moveTime < endSoundThreshold && longDrive && !stopping )
            {
                audioSources[LOOP].loop = false;
                audioSources[CLIP].clip = endSound;
                audioSources[CLIP].PlayDelayed(loopSound.length - audioSources[LOOP].time);
                stopping = true;
            }

            yield return null;

            totDeltaZ += moveSpeed * Time.deltaTime;

            #region Dynamic target update code
            //var closestLane = truckLanes.FindLaneClosestToPoint(playerCamera.transform.position);

            //if (closestLane == 1)
            //{
            //    var currentZPos = transform.position.z - localOffset.localPosition.z;
            //    destinationZ = targetZ + localOffset.localPosition.z;
            //    destinationZ = Mathf.Clamp(destinationZ, currentZPos, truckEndPointZ - localOffset.localPosition.z);

            //    var zDiff = playerCamera.transform.position.z - destinationZ;

            //    if (Mathf.Abs(zDiff) > moveThreshold)
            //    {
            //        targetZ = playerCamera.transform.position.z;
            //        destinationZ = targetZ + localOffset.localPosition.z;
            //        destinationZ = Mathf.Clamp(destinationZ, currentZPos, truckEndPointZ - localOffset.localPosition.z);
            //        targetPos = new Vector3(initialPos.x, initialPos.y, destinationZ);
            //        range = targetPos.z - initialPos.z;
            //    }
            //}
            #endregion
        }
        #endregion

        transform.position = targetPos;

        moving = false;
    }

    //
    // Stops any currently playing sounds
    //
    private void StopSounds()
    {
        foreach (var source in audioSources)
        {
            source.Stop();
        }
    }

    //
    // Counts the movement as unsafe if there are a stack 3 in height of
    //
    private bool IsMovementSafe()
    {
        var isSafe = true;

        foreach (var stock in carryingArea.CarriedStock.Where(s => !s.IsWrapped))
        {
            foreach (var ovrStock in stock.GetStockAbove())
            {
                if (ovrStock.GetStockAbove().Count > 0)
                {
                    isSafe = false;
                    break;
                }
            }

            if (!isSafe)
                break;
        }

        return isSafe;
    }

    //
    // Translates all stock currently carried by truck by given amount
    //
    private bool MoveStock(Vector3 posChange)
    {
        var stockStaying = true;

        foreach (var stock in carryingArea.CarriedStock)
        {
            stock.transform.position += posChange;
            stockStaying = stockStaying && !stock.IsTumbling;
        }

        return stockStaying;
    }
}
