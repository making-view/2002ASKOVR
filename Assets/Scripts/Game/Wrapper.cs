using System.Collections;
using System.Linq;
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
    [SerializeField] [Range(0,1)] float speed = 0.2f;
    [SerializeField] [Range(0, 1)] float requiredAreaFilled = 0.85f;
    [SerializeField] float minPlastic = 1f;
    [SerializeField] float maxPlastic = -8f;

    bool wrapping = false;
    bool unwrapping = false;
    float palletArea = 0.0f;
    Vector3 startPos;
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
        startPos = transform.localPosition;

        var palletCollider = pallet.GetComponent<BoxCollider>();
        palletArea = palletCollider.bounds.size.x * palletCollider.bounds.size.z;
    }

    void Update()
    {
        Debug.Log("Wrapping: " + wrapping);

        if (wrapping && CanWrap())
        {
            transform.position = new Vector3(transform.position.x, 
                transform.position.y + Time.deltaTime * speed, 
                transform.position.z
            );
        }
        else if (unwrapping)
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y - Time.deltaTime * speed,
                transform.position.z
            );
        }
        else
        {
            wrapping = false;
            unwrapping = false;
        }

        if (transform.position.y > topPoint.transform.position.y)
            transform.position = topPoint.transform.position;

        if (transform.position.y < bottomPoint.transform.position.y)
            transform.position = bottomPoint.transform.position;

        if (wrapping || unwrapping)
        {
            foreach (var stock in carryingArea.CarriedStock)
            {
                var isBelow = stock.transform.position.y <= transform.position.y;
                stock.SetWrapped(isBelow);
            }

            var currentPlasticProgress = transform.position.y.Map(
                bottomPoint.transform.position.y, topPoint.transform.position.y,
                minPlastic, maxPlastic
            );

            plasticMaterial.SetFloat("_OpacityGradient", currentPlasticProgress);
        }
    }

    private bool CanWrap()
    {
        var canWrap = false;

        var stockInside = Physics.OverlapBox(boxCollider.transform.position, boxCollider.bounds.extents).Where(c => c.GetComponent<Stock>() != null);

        if (stockInside.Count() > 0)
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

    public void SetUnwrapping(bool unwrapping)
    {
        this.unwrapping = unwrapping;
    }

    public void StopWrapActions()
    {
        unwrapping = false;
        wrapping = false;
    }
}
