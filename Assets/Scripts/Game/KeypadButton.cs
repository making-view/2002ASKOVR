using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ButtonType
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
    [SerializeField] public ButtonType buttonType = ButtonType.Number;

    private PushableButton button = null; 
    private Text text = null;
    private Keypad keypad = null;

    private PickList picklist = null;
    private TutorialKeypad tutorialKeypad = null;
    private GameManager gameManager = null;

    private MeshRenderer meshRenderer = null;
    private Material material = null;
    private Color initColor = Color.white;
    private bool highlighted = false;

    [HideInInspector]
    public bool initialized = false;

    private void OnEnable()
    {
        if (!initialized)
            Initialize();
    }

    //
    // called by parent keypad on start
    //
    public void Initialize()
    {
        keypad = GetComponentInParent<Keypad>();
        button = GetComponentInChildren<PushableButton>();
        text = GetComponentInChildren<Text>();

        picklist = FindObjectOfType<PickList>();
        tutorialKeypad = GetComponentInParent<TutorialKeypad>();
        gameManager = FindObjectOfType<GameManager>();

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        material = meshRenderer.material;
        initColor = material.GetColor("_EmissionColor");

        if (picklist)
            AssignPicklistListeners();
        if (tutorialKeypad)
            AssignTutorialListeners();

        initialized = true;
    }

    public void Highlight(bool highlight)
    {
        if (material == null)
        {
            Debug.LogWarning("material = null - " + gameObject.name);
            return;
        }

        if (highlight && !highlighted)
            StartCoroutine(WhackyWavyLighting());
        else
        {
            StopAllCoroutines();
            highlighted = false;
            material.SetColor("_EmissionColor", initColor);
        }
    }

    private IEnumerator WhackyWavyLighting()
    {
        highlighted = true;

        var intensity = 1.5f;
        if (buttonType.Equals(ButtonType.Number))
            intensity = 2.0f;
        var pow = Mathf.Pow(2, intensity);

        var maxColor = new Color(initColor.r * pow, initColor.g * pow, initColor.b * pow);

        while (true)
        {
            material.SetColor("_EmissionColor", Color.Lerp(initColor, maxColor, Mathf.SmoothStep(0.0f, 1.0f, Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad * 2)))));
            yield return null;
        }
    }

    private void AssignPicklistListeners()
    {
        switch (buttonType)
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

    private void AssignTutorialListeners()
    {
        switch (buttonType)
        {
            case ButtonType.Number:
                button.onButtonPushed.AddListener(tutorialKeypad.Number);
                button.onButtonPushed.AddListener(SendTextToPad);
                break;
            case ButtonType.Confirm:
                button.onButtonPushed.AddListener(tutorialKeypad.Confirm);
                break;
            case ButtonType.Backspace:
                button.onButtonPushed.AddListener(tutorialKeypad.Backspace);
                button.onButtonPushed.AddListener(keypad.Backspace);
                break;
            case ButtonType.Repeat:
                button.onButtonPushed.AddListener(tutorialKeypad.Repeat);
                break;
            case ButtonType.Wrap:
                button.onButtonPushed.AddListener(keypad.ToggleWrapping);
                button.onButtonPushed.AddListener(tutorialKeypad.Plast);
                break;
            case ButtonType.Unwrap:
                button.onButtonPushed.AddListener(keypad.StartUnwrapping);
                button.onButtonReleased.AddListener(keypad.StopUnwrapping);
                button.onButtonPushed.AddListener(tutorialKeypad.CutPlast);
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
