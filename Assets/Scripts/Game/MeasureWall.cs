using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MeasureWall : MonoBehaviour
{
    public int imprecision;

    public Vector3 stockHitPosition = Vector3.zero;
    public Vector3 palletHitPosition = Vector3.zero;

    public IEnumerator Measure()
    {
        var timer = 0.0f;
        var startPos = transform.position;
        var middle = transform.parent.transform;
        var targetPos = new Vector3(middle.position.x, transform.position.y, middle.position.z);

        stockHitPosition = Vector3.zero;
        palletHitPosition = Vector3.zero;

        while (timer <= 2.0f && (stockHitPosition == Vector3.zero || palletHitPosition == Vector3.zero))
        {
            timer += Time.deltaTime;

            float step = Mathf.SmoothStep(0, 1, timer / 2);

            transform.position = Vector3.Lerp(startPos, targetPos, step);

            yield return null;
        }

        if (stockHitPosition == Vector3.zero || palletHitPosition == Vector3.zero)
        {
            imprecision = 999;
        }
        else
        {
            imprecision = (int)((stockHitPosition - palletHitPosition).magnitude * 100);
        }

        foreach (var text in GetComponentsInChildren<Text>())
        {
            text.text = imprecision.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Pallet"))
        {
            palletHitPosition = transform.position;
        }
        else if (other.gameObject.GetComponent<Stock>())
        {
            stockHitPosition = transform.position;
        }
    }
}
