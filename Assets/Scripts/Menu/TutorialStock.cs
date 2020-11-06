using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stock))]
public class TutorialStock : MonoBehaviour
{
    [SerializeField] private bool countStacking = false;
    [SerializeField] private float grabTime = 3.0f;

    private Tutorial tutorial = null;
    private Stock stock = null;

    private float grabbedTimer = 0.0f;
    private int prevStockAbove = 0;

    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
        stock = GetComponent<Stock>();
    }

    void Update()
    {
        if (!tutorial.waitingForNextEvent)
        {
            if (stock.IsGrabbed)
                grabbedTimer += Time.deltaTime;
            else
                grabbedTimer = 0.0f;

            if (grabbedTimer >= grabTime)
                tutorial.DoTask(Tutorial.Task.Grab);

            if (stock.HasRotatedSinceLastQuery)
                tutorial.DoTask(Tutorial.Task.RotateStock);

            var currStockAbove = stock.GetStockAbove().Count;

            if (countStacking && currStockAbove > prevStockAbove && !stock.IsGrabbed)
            {
                for (int i = 0; i < currStockAbove - prevStockAbove; i++)
                    tutorial.DoTask(Tutorial.Task.Stack);

                prevStockAbove = currStockAbove;
            }
        }
    }
}
