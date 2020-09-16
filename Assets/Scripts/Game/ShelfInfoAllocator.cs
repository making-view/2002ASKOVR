using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShelfInfoAllocator : MonoBehaviour
{
    [SerializeField] private string stockCode = "000";
    [SerializeField] private int shelfNumber = 0;

    private void Awake()
    {
        var collider = GetComponent<BoxCollider>();
        var center = collider.transform.position + collider.center;
        var halfExtents = collider.size / 2;

        var objectsInCollider = Physics.OverlapBox(center, halfExtents)
            .Where(s => s.GetComponent<Stock>() != null)
            .Select(s => s.GetComponent<Stock>()).ToList();

        foreach (var item in objectsInCollider)
        {
            item.StockCode = stockCode;
            item.ShelfNumber = shelfNumber;
        }

        PickList.shelves.Add(new ShelfInfo() { 
            shelfNo = shelfNumber, 
            amount = objectsInCollider.Count, 
            stockCode = stockCode 
        });
    }
}
