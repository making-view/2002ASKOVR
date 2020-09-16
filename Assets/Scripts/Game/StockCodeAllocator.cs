using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StockCodeAllocator : MonoBehaviour
{
    [SerializeField] private string stockCode = "000";
    [SerializeField] private int shelfNumber = 0;

    private void Awake()
    {
        var collider = GetComponent<BoxCollider>();
        var center = collider.transform.position + collider.center;
        var halfExtents = collider.size / 2;

        var objectsInCollider = Physics.OverlapBox(center, halfExtents);

        foreach (var item in objectsInCollider)
        {
            var stock = item.gameObject.GetComponent<Stock>();

            if (stock)
            {
                stock.StockCode = stockCode;
                stock.ShelfNumber = shelfNumber;
            }
        }
    }
}
