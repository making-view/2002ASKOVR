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

    public string Command { get; private set; } = "";

    private void Start()
    {
        Command = "";
        display.text = Command;
    }

    public void AddToCommand(string add)
    {
        if(Command.Length < 4)
            Command += add;

        display.text = Command;
    }

    public void SendCommand()
    {
        pickList?.ReceiveCommand(Command.Trim());
        Command = "";
        display.text = Command;
    }

    public void Repeat()
    {
        pickList?.ReceiveCommand("Repeat");
    }

    public void Backspace()
    {
        Command = Command.Substring(0, Mathf.Clamp(Command.Length - 1, 0, 999));
        display.text = Command;
    }

    public void ToggleWrapping()
    {
        wrapper?.ToggleWrapping();
    }

    public void StartUnwrapping()
    {
        wrapper?.SetUnwrapping(true);
    }

    public void StopUnwrapping()
    {
        wrapper?.SetUnwrapping(false);
    }

    public void StopWrapActions()
    {
        wrapper?.StopWrapActions();
    }
}
