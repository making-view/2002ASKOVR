using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
    [SerializeField] bool wrapping = false;
    [SerializeField] [Range(0,1)] float speed = 0.2f;
    private List<GameObject> stockInside = null;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        stockInside = new List<GameObject>();
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (wrapping)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + Time.deltaTime * speed, transform.localPosition.z);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Stock>() != null)
            stockInside.Add(collider.gameObject);

        Debug.Log(collider.gameObject.name + " entered wrapping volume");
    }

    private void OnTriggerExit(Collider collider)
    {
        if (stockInside.Remove(collider.gameObject))
        {
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collider.gameObject.transform.parent = transform.parent;
        }

        if (stockInside.Count == 0)
            wrapping = false;

        Debug.Log(collider.gameObject.name + " left wrapping volume");
    }

    public void SetWrapping(bool wrapping)
    {
        this.wrapping = wrapping;
    }


    public void ResetWrapping()
    {
        transform.localPosition = startPos;
    }
}
