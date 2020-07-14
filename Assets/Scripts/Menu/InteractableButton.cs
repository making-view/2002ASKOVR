using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using OculusSampleFramework;

[RequireComponent(typeof(ButtonController))]
[RequireComponent(typeof(Image))]
public class InteractableButton : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;

    [Space]
    public UnityEvent OnClick;

    private ButtonController controller;
    private Image buttonImage;

    private bool isPressed = false;

    void Start()
    {
        controller = GetComponent<ButtonController>();
        buttonImage = GetComponent<Image>();

        controller.InteractableStateChanged.AddListener(InteractableStateChanged);
    }

    void InteractableStateChanged(InteractableStateArgs state)
    {
        switch (state.NewInteractableState)
        {
            case InteractableState.ProximityState:
            case InteractableState.ContactState:
                OnHighlighted();
                break;
            case InteractableState.ActionState:
                OnPressed();
                break;
            case InteractableState.Default:
                OnLostFocus();
                break;
        }
    }

    private void OnHighlighted()
    {
        buttonImage.color = highlightedColor;

        if (isPressed)
        {
            OnClick.Invoke();
        }

        isPressed = false;
    }

    private void OnPressed()
    {
        buttonImage.color = pressedColor;

        isPressed = true;
    }

    private void OnLostFocus()
    {
        buttonImage.color = normalColor;

        isPressed = false;
    }
}
