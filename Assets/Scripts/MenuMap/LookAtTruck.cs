using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTruck : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    Vector3 startForward;
    [SerializeField] float maxAngle = 45.0f;
    [SerializeField] bool debugLog = false;
    [SerializeField] AudioSource moo = null;
    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
            target = GameObject.Find("FrontMount");

        startForward = transform.forward;
        //Debug.Log("start forward: " + startForward.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        var targetVector = (target.transform.position - transform.position).normalized;

        var angle = Vector3.Angle(startForward, targetVector);

        if (debugLog)
            Debug.Log("angle b4: " + angle);

        if (angle > maxAngle)
        {
            targetVector = Vector3.Lerp(startForward, targetVector, maxAngle / angle);

            if (debugLog)
                Debug.Log("angle af: " + Vector3.Angle(startForward, targetVector));

            transform.LookAt(transform.position + targetVector);
        }
        else
            transform.LookAt(target.transform);

        if(moo != null && moo.enabled == false && angle < 42)
        {
            moo.enabled = true;
        }
    }
}
