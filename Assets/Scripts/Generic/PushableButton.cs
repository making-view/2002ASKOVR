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

    void Start()
    {
        rigidboody = GetComponent<Rigidbody>();
        midPoint = (topPoint.transform.localPosition + bottomPoint.transform.localPosition) / 2;
    }

    void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rigidboody.velocity);
        localVelocity.x = 0;
        localVelocity.z = 0;

        rigidboody.velocity = transform.TransformDirection(localVelocity);

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
