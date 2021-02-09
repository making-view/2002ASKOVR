using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float verticalRange = 1.0f;
    public float spin = 10.0f;

    private float offset;

    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        offset = Random.Range(0, 2 * Mathf.PI);
        position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = position.y + Mathf.Sin(Time.timeSinceLevelLoad + offset) * verticalRange;
        
        if(spin != 0)
        {
            float newRot = transform.rotation.eulerAngles.y + spin * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, newRot, 0);
        }

        transform.localPosition = new Vector3(position.x, newY, position.z);
    }
}
