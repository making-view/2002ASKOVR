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
    [Tooltip ("leave null if no 3D object to highlight")]
    [SerializeField] private Highlight highlightObject = null;

    [Space]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightedColor = Color.white;
    [SerializeField] private Color pressedColor = Color.white;

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
        if (highlightObject != null)
            highlightObject.StartHighlight();

        buttonImage.color = highlightedColor;

        if (isPressed)
        {
            OnClick.Invoke();
        }

        isPressed = false;
    }

    public void OnPressed()
    {
        if (highlightObject != null)
            highlightObject.StopHighlight();

        buttonImage.color = pressedColor;

        isPressed = true;
    }

    private void OnLostFocus()
    {
        if (highlightObject != null)
            highlightObject.StopHighlight();

        buttonImage.color = normalColor;

        isPressed = false;
    }
}
