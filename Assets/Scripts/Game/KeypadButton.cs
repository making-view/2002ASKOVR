using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ButtonType
{
    Number,
    Confirm,
    Backspace,
    Repeat,
    Wrap,
    Unwrap
}

public class KeypadButton : MonoBehaviour
{
    [SerializeField] ButtonType buttonType;

    private PushableButton button = null; 
    private Text text = null;
    private Keypad keypad = null;

    private PickList picklist = null;
    private GameManager gameManager = null;

    //
    // Assigns this button to the correct action on the correct keypad
    //
    private void Start()
    {
        keypad = GetComponentInParent<Keypad>();
        button = GetComponentInChildren<PushableButton>();
        text = GetComponentInChildren<Text>();

        picklist = FindObjectOfType<PickList>();
        gameManager = FindObjectOfType<GameManager>();

        switch(buttonType)
        {
            case ButtonType.Number:
                button.onButtonPushed.AddListener(SendTextToPad);
                break;
            case ButtonType.Confirm:
                button.onButtonPushed.AddListener(ConfirmButton);
                break;
            case ButtonType.Backspace:
                button.onButtonPushed.AddListener(keypad.Backspace);
                break;
            case ButtonType.Repeat:
                button.onButtonPushed.AddListener(keypad.Repeat);
                break;
            case ButtonType.Wrap:
                button.onButtonPushed.AddListener(keypad.ToggleWrapping);
                break;
            case ButtonType.Unwrap:
                button.onButtonPushed.AddListener(keypad.StartUnwrapping);
                button.onButtonReleased.AddListener(keypad.StopUnwrapping);
                break;
        }
    }

    private void ConfirmButton()
    {
        if (picklist.gameObject.activeSelf)
            keypad.SendCommand();
        else
            gameManager.ReadyForReport = true;
    }

    private void SendTextToPad()
    {
        keypad.AddToCommand(text.text);
    }

    private void OnDisable()
    {
        keypad.StopWrapActions();
    }
}
