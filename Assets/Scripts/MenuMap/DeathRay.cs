using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRay : MonoBehaviour
{
    GameObject target = null;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("FrontMount");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
    }
}
