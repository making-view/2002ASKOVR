using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stock))]
public class TutorialStock : MonoBehaviour
{
    [SerializeField] private float grabTime = 3.0f;

    private Tutorial tutorial = null;
    private Stock stock = null;

    private float grabbedTimer = 0.0f;

    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
        stock = GetComponent<Stock>();
    }

    void Update()
    {
        if (stock.IsGrabbed)
            grabbedTimer += Time.deltaTime;
        else
            grabbedTimer = 0.0f;

        if (grabbedTimer >= grabTime)
            tutorial.DoTask(Tutorial.Task.Grab);

        if (stock.HasRotatedSinceLastQuery)
            tutorial.DoTask(Tutorial.Task.RotateStock);
    }
}
