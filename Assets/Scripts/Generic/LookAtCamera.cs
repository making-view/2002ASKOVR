using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtCamera : MonoBehaviour
{
    private GameObject playerCamera;
    [SerializeField] bool targetYposition = false;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = Vector3.zero;

        if (targetYposition)
        {
            targetPostition = playerCamera.transform.position;
        }
        else
        {
            targetPostition = new Vector3(playerCamera.transform.position.x,
                               this.transform.position.y,
                               playerCamera.transform.position.z);
        }
        

        this.transform.LookAt(targetPostition);
    }
}
