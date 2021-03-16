using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToVolume : MonoBehaviour
{
    [SerializeField] Collider volume = null;
    
    Rigidbody rigidBodyComponent = null;
    Vector3 startingPosition;
    Quaternion startingRotation;

    private bool insideVolume = true;

    void OnEnable()
    {
        rigidBodyComponent = transform.parent.GetComponent<Rigidbody>();
        startingPosition = transform.parent.transform.position;
        startingRotation = transform.parent.transform.rotation;

        if (volume == null)
            volume = GameObject.Find("ReturnVolume").GetComponent<Collider>();

        Debug.Log(gameObject.name + "'s return volume set to " + volume.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!rigidBodyComponent.isKinematic && other.gameObject.Equals(volume.gameObject))
        {
            StartCoroutine(Timeout());
            Debug.Log(transform.parent.name + " exiting volume: " + volume.gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!rigidBodyComponent.isKinematic && other.gameObject.Equals(volume.gameObject))
        {
            insideVolume = true;
            Debug.Log(transform.parent.name + " entering volume: " + volume.gameObject.name);
        }
    }


    private IEnumerator Timeout()
    {
        insideVolume = false;

        yield return new WaitForSeconds(2.0f);

        if (!insideVolume)
        {
            Debug.Log(transform.parent.name + " is outside, teleporting");

            rigidBodyComponent.velocity = Vector3.zero;
            rigidBodyComponent.angularVelocity = Vector3.zero;

            transform.parent.transform.position = startingPosition + new Vector3(0.0f, 0.2f, 0.0f);
            transform.parent.transform.rotation = startingRotation;
        }
        else
            Debug.Log(transform.parent.name + " is still inside. not teleporting");
    }
}
