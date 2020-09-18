using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateKeypad : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject keypad = null;
    [SerializeField] private bool right;

    private OVRInput.Button button;
    void Start()
    {
        if (right)
            button = OVRInput.Button.Two;
        else
            button = OVRInput.Button.Four;
    }

    // Update is called once per frame
    void Update()
    {
        keypad.gameObject.SetActive(OVRInput.Get(button));
    }
}
