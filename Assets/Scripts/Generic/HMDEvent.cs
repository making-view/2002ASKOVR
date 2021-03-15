using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HMDEvent : MonoBehaviour
{

    [SerializeField] UnityEvent onUnmounted = null;
    [SerializeField] UnityEvent onMounted = null;

    [SerializeField] bool onlyInMenu = false;
    [SerializeField] private GameObject menu = null;

    private void Start()
    {
        OVRManager.HMDUnmounted += UnmountedEvent;
        OVRManager.HMDMounted += MountedEvent;
    }

    private void UnmountedEvent ()
    {
        if(menu.activeInHierarchy)
            onUnmounted.Invoke();
    }

    private void MountedEvent()
    {
        if (!onlyInMenu || menu.activeInHierarchy)
            onMounted.Invoke();
    }
}
