using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeypadButton : MonoBehaviour
{
    private PushableButton button = null; 
    private Text text = null;
    private Keypad keypad = null;

    private void Start()
    {
        keypad = FindObjectOfType<Keypad>();
        button = GetComponentInChildren<PushableButton>();
        text = GetComponentInChildren<Text>();

        if (text.text.ToLower().Equals("bekreft"))
            button.onButtonPushed.AddListener(keypad.SendCommand);
        else if (text.text.ToLower().Equals("gjenta"))
            button.onButtonPushed.AddListener(keypad.Repeat);
        else if (text.text.ToLower().Equals("del"))
            button.onButtonPushed.AddListener(keypad.Backspace);
        else
            button.onButtonPushed.AddListener(SendTextToPad);
    }

    private void SendTextToPad()
    {
        keypad.AddToCommand(text.text);
    }
}
