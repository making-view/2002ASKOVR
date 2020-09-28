using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MeasureWall : MonoBehaviour
{
    [SerializeField] private GameObject visualScanner = null;

    public int imprecision;

    public Vector3 stockHitPosition = Vector3.zero;
    public Vector3 palletHitPosition = Vector3.zero;

    public IEnumerator Measure()
    {
        var timer = 0.0f;

        var startPos = transform.position;
        var middle = transform.parent.parent.transform;
        var targetPos = new Vector3(middle.position.x, transform.position.y, middle.position.z);

        visualScanner.gameObject.SetActive(true);

        stockHitPosition = Vector3.zero;
        palletHitPosition = Vector3.zero;

        while (timer <= 2.0f && (stockHitPosition == Vector3.zero || palletHitPosition == Vector3.zero))
        {
            timer += Time.deltaTime;

            float step = Mathf.SmoothStep(0, 1, timer / 2);

            var currStockHitPosition = stockHitPosition;

            transform.position = Vector3.Lerp(startPos, targetPos, step);

            if (currStockHitPosition == Vector3.zero)
            {
                visualScanner.transform.position = transform.position;
            }

            yield return null;
        }

        if (stockHitPosition == Vector3.zero || palletHitPosition == Vector3.zero)
        {
            imprecision = (int)((targetPos - palletHitPosition).magnitude * 100);
        }
        else
        {
            imprecision = (int)((stockHitPosition - palletHitPosition).magnitude * 100);
        }

        foreach (var text in visualScanner.GetComponentsInChildren<Text>())
        {
            text.text = imprecision.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Pallet"))
        {
            if (palletHitPosition == Vector3.zero)
                palletHitPosition = transform.position;
        }
        else if (other.gameObject.GetComponent<Stock>() || other.gameObject.transform.parent.gameObject.GetComponent<Stock>())
        {
            if (stockHitPosition == Vector3.zero)
                stockHitPosition = transform.position;
        }
    }
}
