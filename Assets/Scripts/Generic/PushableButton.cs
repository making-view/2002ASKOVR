using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushableButton : MonoBehaviour
{
    public UnityEvent onButtonPushed;

    [SerializeField] private GameObject topPoint = null;
    [SerializeField] private GameObject bottomPoint = null;

    private bool pressed = false;
    private Vector3 midPoint;

    void Start()
    {
        midPoint = (topPoint.transform.position + bottomPoint.transform.position) / 2;
    }

    void Update()
    {
        if (transform.position.y > topPoint.transform.position.y)
        {
            transform.position = topPoint.transform.position;
        }

        if (transform.position.y < bottomPoint.transform.position.y)
        {
            transform.position = bottomPoint.transform.position;

            if (!pressed)
            {
                onButtonPushed.Invoke();
                pressed = true;
            }
        }

        if (transform.position.y > midPoint.y)
        {
            pressed = false;
        }
    }
}
