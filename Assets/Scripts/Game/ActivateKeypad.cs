using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateKeypad : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject keypad = null;
    [SerializeField] private bool right = false;

    public bool IsActive
    {
        get
        {
            return OVRInput.Get(button);
        }
    }

    private OVRInput.Button button;
    void Start()
    {
        if (right)
            button = OVRInput.Button.One;
        else
            button = OVRInput.Button.Three;
    }

    // Update is called once per frame
    void Update()
    {
        keypad.gameObject.SetActive(OVRInput.Get(button));
    }
}
