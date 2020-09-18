using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// Interacts with PickList to execute game actions
//
public class Keypad : MonoBehaviour
{
    [SerializeField] Text display = null;
    [SerializeField] PickList pickList = null;
    [SerializeField] Wrapper wrapper = null;

    private string command = "";

    private void Start()
    {
        command = "";
        display.text = command;
    }

    public void AddToCommand(string add)
    {
        command += add;
        display.text = command;
    }

    public void SendCommand()
    {
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

    public void StartWrapping()
    {
        wrapper.SetWrapping(true);
    }

    public void StopWrapping()
    {
        wrapper.SetWrapping(false);
    }
}
