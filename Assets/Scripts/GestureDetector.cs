using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public OVRSkeleton skeleton;
    public float threshold = 0.1f;
    public bool allowGestureCreation = false;
    public List<Gesture> gestures;

    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    void Update()
    {
        if (allowGestureCreation && Input.GetKeyDown(KeyCode.Space))
        {
            SaveGesture();
        }

        Gesture currentGesture = DetectGesture();
        bool gestureDetected = !currentGesture.Equals(new Gesture());

        if (gestureDetected && !currentGesture.Equals(previousGesture))
        {
            previousGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
        }
    }

    // Saves the current fingerpositions as a new Gesture in gestures list
    void SaveGesture()
    {
        Gesture gesture = new Gesture();
        gesture.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBones)
        {
            // Finger position relative to root
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        gesture.fingerData = data;
        gestures.Add(gesture);
    }

    // If there are any saved gestures similar to current hand pose, returns the most similar one
    Gesture DetectGesture()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool discarded = false;

            for (int boneNo = 0; boneNo < fingerBones.Count; boneNo++)
            {
                Vector3 thisBonePos = skeleton.transform.InverseTransformPoint(fingerBones[boneNo].Transform.position);
                float distance = Vector3.Distance(thisBonePos, gesture.fingerData[boneNo]);

                if (distance > threshold)
                {
                    discarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!discarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
