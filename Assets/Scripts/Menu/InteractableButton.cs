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
                buttonImage.color = highlightedColor;
                break;
            case InteractableState.ContactState:
                buttonImage.color = highlightedColor;
                break;
            case InteractableState.ActionState:
                buttonImage.color = pressedColor;
                break;
            case InteractableState.Default:
                buttonImage.color = normalColor;
                break;
        }
    }
}
