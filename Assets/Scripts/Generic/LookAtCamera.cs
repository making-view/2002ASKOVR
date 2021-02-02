using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtCamera : MonoBehaviour
{
    private GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(camera.transform.position.x,
                               this.transform.position.y,
                               camera.transform.position.z);

        this.transform.LookAt(targetPostition);
    }
}
