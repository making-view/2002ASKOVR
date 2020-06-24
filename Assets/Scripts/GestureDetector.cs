using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum PoseName
{
    None,
    ThumbsUp,
    JazzHand,
    Fist
};

[System.Serializable]
public struct Gesture
{
    public string name;
    public PoseName poseName;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public OVRSkeleton skeleton = null;
    public float threshold = 0.1f;
    public bool allowGestureCreation = false;
    public List<Gesture> gestures;

    private List<OVRBone> fingerBones;
    private Gesture previousGesture;
    private Gesture currentGesture;

    private float gestureChangeThreshold = 0.15f;
    private float timeSinceGestureChange = 0.0f;

    public bool IsAnyGestureActive
    {
        get
        {
            return skeleton.gameObject.GetComponent<SkinnedMeshRenderer>().enabled
                && currentGesture.poseName != PoseName.None;
        }
    }

    public bool IsGestureActive(PoseName poseName)
    {
        return currentGesture.poseName == poseName;
    }

    public bool IsGestureActive(string name)
    {
        return currentGesture.name == name;
    }

    void Start()
    {
        StartCoroutine(GetFingerBones());
        previousGesture = new Gesture();

        //
        // Detects if the gesture list has several entries using the same PoseName
        //
        if (gestures.GroupBy(g => g.poseName).Any(gg => gg.Count() > 1))
        {
            throw new DuplicatePoseNameException("A PoseName can only be used for one gesture at a time, please remove any duplicates");
        }
    }

    void Update()
    {
        if (allowGestureCreation && fingerBones.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            SaveGesture();
        }

        currentGesture = DetectGesture();
        bool gestureDetected = currentGesture.poseName != PoseName.None;

        //
        // Counts up while the current gesture is different from the previously activated one
        // Resets when they're the same
        //
        if (currentGesture.poseName != previousGesture.poseName)
            timeSinceGestureChange += Time.deltaTime;
        else
            timeSinceGestureChange = 0.0f;

        //
        // Invoke and change gesture if previous one was None or if current gesture has been
        // different from previous gesture for longer than gestureChangeThreshold
        //
        if ((gestureDetected && previousGesture.poseName == PoseName.None)
            || (timeSinceGestureChange > gestureChangeThreshold))
        {
            currentGesture.onRecognized.Invoke();
            previousGesture = currentGesture;
        }
    }

    //
    // Saves the current fingerpositions as a new Gesture in gestures list
    //
    void SaveGesture()
    {
        Gesture gesture = new Gesture();
        gesture.name = "New Gesture";
        gesture.poseName = PoseName.None;
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBones)
        {
            //
            // Finger position relative to root (ie localPosition only, rotation is ignored)
            //
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        gesture.fingerData = data;
        gestures.Add(gesture);
    }

    //
    // If there are any saved gestures similar to current hand pose, returns the most similar one
    //
    Gesture DetectGesture()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool skipped = false;

            //
            // Calculates and sums up the difference between current bone position and gesture bone position
            // Skips this gesture if any bone position is too different to that of the gesture
            //
            for (int boneNo = 0; boneNo < fingerBones.Count; boneNo++)
            {
                Vector3 thisBonePos = skeleton.transform.InverseTransformPoint(fingerBones[boneNo].Transform.position);
                float distance = Vector3.Distance(thisBonePos, gesture.fingerData[boneNo]);

                if (distance > threshold)
                {
                    skipped = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!skipped && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }

    //
    // Populates fingerbones list, happens first time the Quest detects hands in-game
    //
    IEnumerator GetFingerBones()
    {
        do
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
            yield return null;
        } while (fingerBones.Count <= 0);
    }
}
