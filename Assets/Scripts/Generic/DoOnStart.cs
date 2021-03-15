using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent onStart = null;


    // Start is called before the first frame update
    void Start()
    {
        onStart.Invoke();
    }
}
