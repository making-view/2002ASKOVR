using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OculusSampleFramework;

[RequireComponent(typeof(ButtonController))]
[RequireComponent(typeof(Button))]
public class InteractableUIButton : MonoBehaviour
{
    private ButtonController controller;
    private Button uiButton;

    void Start()
    {
        controller = GetComponent<ButtonController>();
        uiButton = GetComponent<Button>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
