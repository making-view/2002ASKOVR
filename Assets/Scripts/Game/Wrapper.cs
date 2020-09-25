using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject pallet = null;
    [SerializeField] GameObject plastic = null;
    [SerializeField] CarryingArea carryingArea = null;
    [SerializeField] GameObject topPoint = null;
    [SerializeField] GameObject bottomPoint = null;

    [Header("Settings")]
    [SerializeField] bool wrapping = false;
    [SerializeField] [Range(0,1)] float speed = 0.2f;
    [SerializeField] [Range(0, 1)] float requiredAreaFilled = 0.85f;
    [SerializeField] float minPlastic = 1f;
    [SerializeField] float maxPlastic = -8f;

    List<GameObject> stockInside = null;
    Vector3 startPos;
    float palletArea = 0.0f;
    BoxCollider boxCollider;
    Material plasticMaterial;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        plasticMaterial = plastic.GetComponent<MeshRenderer>().material;
        plasticMaterial.SetFloat("_OpacityGradient", minPlastic);
    }

    void Start()
    {
        stockInside = new List<GameObject>();
        startPos = transform.localPosition;

        var palletCollider = pallet.GetComponent<BoxCollider>();
        palletArea = palletCollider.bounds.size.x * palletCollider.bounds.size.z;
    }

    void Update()
    {
        Debug.Log("Wrapping: " + wrapping);

        if (wrapping && CanWrap())
        {
            Debug.Log("CanWrap");

            transform.localPosition = new Vector3(transform.localPosition.x, 
                transform.localPosition.y + Time.deltaTime * speed, 
                transform.localPosition.z
            );

            foreach(var stock in carryingArea.CarriedStock)
            {
                var isBelow = stock.transform.position.y <= transform.position.y;
                stock.SetWrapped(isBelow);
            }

            var currentPlasticProgress = (transform.position.y - boxCollider.bounds.size.y / 2).Map(
                bottomPoint.transform.position.y, topPoint.transform.position.y, 
                minPlastic, maxPlastic
            );

            plasticMaterial.SetFloat("_OpacityGradient", currentPlasticProgress);
        }
        else
        {
            wrapping = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Stock>() != null)
            stockInside.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        stockInside.Remove(collider.gameObject);
    }

    private bool CanWrap()
    {
        var canWrap = false;

        if (stockInside.Count > 0)
        {
            var totArea = 0.0f;

            foreach (var stock in stockInside)
            {
                var stockCollider = stock.GetComponent<BoxCollider>();
                totArea += stockCollider.bounds.size.x * stockCollider.bounds.size.z;
            }

            canWrap = totArea >= (palletArea * requiredAreaFilled);
        }

        return canWrap;
    }

    public void ToggleWrapping()
    {
        wrapping = !wrapping;
    }

    public void StopWrapping()
    {
        wrapping = false;
    }


    public void ResetWrapping()
    {
        transform.localPosition = startPos;
    }
}
