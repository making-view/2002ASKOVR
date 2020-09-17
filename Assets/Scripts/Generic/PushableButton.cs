using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushableButton : MonoBehaviour
{
    public UnityEvent onButtonPushed;

    [SerializeField] private GameObject topPoint = null;
    [SerializeField] private GameObject bottomPoint = null;

    Rigidbody rigidboody = null;

    private bool pressed = false;
    private Vector3 midPoint;

    private Vector3 initLocalPos;

    void Start()
    {
        rigidboody = GetComponent<Rigidbody>();
        midPoint = (topPoint.transform.localPosition + bottomPoint.transform.localPosition) / 2;
        initLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = new Vector3(initLocalPos.x, transform.localPosition.y, initLocalPos.z);

        if (transform.localPosition.y > topPoint.transform.localPosition.y)
        {
            transform.localPosition = topPoint.transform.localPosition;
        }

        if (transform.localPosition.y < bottomPoint.transform.localPosition.y)
        {
            transform.localPosition = bottomPoint.transform.localPosition;

            if (!pressed)
            {
                onButtonPushed.Invoke();
                pressed = true;
            }
        }

        if (transform.localPosition.y > midPoint.y)
        {
            pressed = false;
        }
    }
}
