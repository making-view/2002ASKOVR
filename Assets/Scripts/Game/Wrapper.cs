using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject pallet = null;
    [SerializeField] GameObject plastic = null;
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
    Material plasticMaterial;

    private void Awake()
    {
        plasticMaterial = plastic.GetComponent<MeshRenderer>().material;
        plasticMaterial.SetFloat("_OpacityGradient", minPlastic);
    }

    // Start is called before the first frame update
    void Start()
    {
        stockInside = new List<GameObject>();
        startPos = transform.localPosition;

        var palletCollider = pallet.GetComponent<BoxCollider>();
        palletArea = palletCollider.bounds.size.x * palletCollider.bounds.size.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (wrapping && CanWrap())
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + Time.deltaTime * speed, transform.localPosition.z);

            var currentPlasticProgress = transform.position.y.Map(
                bottomPoint.transform.position.y, topPoint.transform.position.y, 
                minPlastic, maxPlastic
            );

            plasticMaterial.SetFloat("_OpacityGradient", currentPlasticProgress);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Stock>() != null)
            stockInside.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (stockInside.Remove(collider.gameObject))
        {
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collider.gameObject.transform.parent = transform.parent;
        }
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

    public void SetWrapping(bool wrapping)
    {
        this.wrapping = wrapping;
    }


    public void ResetWrapping()
    {
        transform.localPosition = startPos;
    }
}
