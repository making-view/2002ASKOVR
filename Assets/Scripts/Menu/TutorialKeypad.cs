using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TutorialKeypad : MonoBehaviour
{
    public string commandToLookFor = "123";

    private Tutorial tutorial = null;
    private Keypad keypad = null;
    private List<KeypadButton> buttons;

    private ButtonType prevHighlightKey = ButtonType.Wrap;
    private bool prevStockCodeHighlight = false;
    private bool prevStockAmountHighlight = false;

    private bool hasInitialized = false;

    private void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
        keypad = GetComponent<Keypad>();
        buttons = GetComponentsInChildren<KeypadButton>().ToList();

        hasInitialized = true;
    }

    private void Update()
    {
        HandleHighlight();
    }

    public void Confirm()
    {
        if (keypad.Command.Equals(commandToLookFor))
        {
            tutorial.DoTask(Tutorial.Task.ConfirmKey);
        }
    }

    public void Repeat()
    {
        tutorial.DoTask(Tutorial.Task.RepeatKey);

    }

    public void Backspace()
    {
        tutorial.DoTask(Tutorial.Task.BackspaceKey);
    }

    public void Number()
    {
        tutorial.DoTask(Tutorial.Task.NumberKey);
    }

    private void OnEnable()
    {
        if (!hasInitialized)
        {
            tutorial = FindObjectOfType<Tutorial>();
            keypad = GetComponent<Keypad>();
            buttons = GetComponentsInChildren<KeypadButton>().ToList();

            hasInitialized = true;
            HandleHighlight();
        }
    }

    private void HandleHighlight()
    {
        if (tutorial.HighlightStockCode && !prevStockCodeHighlight)
        {
            prevStockCodeHighlight = true;

            foreach (var button in buttons)
            {
                if (commandToLookFor.Contains(button.name))
                    button.Highlight(true);
                else
                    button.Highlight(false);
            }
        }
        else if (tutorial.HighlightStockAmount && !prevStockAmountHighlight)
        {
            prevStockAmountHighlight = true;

            foreach (var button in buttons)
            {
                if (button.name.Equals("5"))
                    button.Highlight(true);
                else
                    button.Highlight(false);
            }
        }
        else if (tutorial.HighlightKey.HasValue && !prevHighlightKey.Equals(tutorial.HighlightKey.Value))
        {
            prevHighlightKey = tutorial.HighlightKey.Value;

            foreach (var button in GetComponentsInChildren<KeypadButton>())
            {
                button.Highlight(button.buttonType.Equals(tutorial.HighlightKey.Value));
            }
        }
    }
}
