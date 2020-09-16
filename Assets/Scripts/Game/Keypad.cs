using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    [SerializeField] PickList pickList;
    [SerializeField] Text display;

    private string command = "";

    private void Start()
    {
        command = "";
        display.text = command;
    }

    public void AddToCommand(string add)
    {
        command += add;
        Debug.Log("Numpad, current command: " + command);
        display.text = command;
    }

    public void SendCommand()
    {
        Debug.Log("Sending command");
        pickList.ReceiveCommand(command.Trim());
        command = "";
        display.text = command;
    }

    public void Repeat()
    {
        pickList.ReceiveCommand("Repeat");
    }

    public void Backspace()
    {
        command = command.Substring(0, Mathf.Clamp(command.Length - 1, 0, 999));
        display.text = command;
    }
}
