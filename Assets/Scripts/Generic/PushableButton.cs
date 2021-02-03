using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushableButton : MonoBehaviour
{
    public UnityEvent onButtonPushed;
    public UnityEvent onButtonReleased;

    [SerializeField] private GameObject topPoint = null;
    [SerializeField] private GameObject bottomPoint = null;

    Rigidbody rigidboody = null;

    private bool pressed = false;
    private Vector3 midPoint;

    private Vector3 initLocalPos;

    [SerializeField] private KeyCode debugPress = KeyCode.None;

    public bool IsButtonPressed()
    {
        return pressed;
    }

    void Start()
    {
        rigidboody = GetComponent<Rigidbody>();
        midPoint = (topPoint.transform.localPosition + bottomPoint.transform.localPosition) / 2;
        initLocalPos = transform.localPosition;

        if (debugPress != KeyCode.None)
            StartCoroutine(DebugKeyCheck());
    }

    private IEnumerator DebugKeyCheck()
    {
        while(true)
        {
            if (Input.GetKeyDown(debugPress))
            {
                onButtonPushed.Invoke();
                Debug.Log(gameObject.name + "'s debug putton pushed");
            }
            yield return null;
        }
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
            if (pressed)
            {
                onButtonReleased.Invoke();
            }

            pressed = false;
        }
    }

    private void OnDisable()
    {
        transform.position = topPoint.transform.position;
    }
}
