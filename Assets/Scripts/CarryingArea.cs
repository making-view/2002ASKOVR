using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingArea : MonoBehaviour
{
    private List<Stock> _carriedStock;
    public IEnumerable<Stock> CarriedStock
    {
        get
        {
            foreach (var stock in _carriedStock) yield return stock;
        }
    }

    void Start()
    {
        _carriedStock = new List<Stock>();        
    }

    private void OnTriggerEnter(Collider other)
    {
        var stock = other.GetComponent<Stock>();

        if (stock != null)
        {
            _carriedStock.Add(stock);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var stock = other.GetComponent<Stock>();

        if (stock != null)
        {
            _carriedStock.Remove(stock);
        }
    }
}
