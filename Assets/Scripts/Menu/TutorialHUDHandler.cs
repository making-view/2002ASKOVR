using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHUDHandler : MonoBehaviour
{
    [SerializeField] private Image rightGraphic = null;
    [SerializeField] private Image leftGraphic = null;

    [SerializeField] private Sprite none = null;
    [SerializeField] private Sprite grip = null;
    [SerializeField] private Sprite one = null;
    [SerializeField] private Sprite stick = null;

    private Settings settings = null;

    void Start()
    {
        settings = FindObjectOfType<Settings>();
    }

    public void ShowHUD()
    {
        rightGraphic.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void HideHUD()
    {
        rightGraphic.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void ShowNoControls()
    {
        ShowHUD();

        rightGraphic.sprite = none;
        leftGraphic.sprite = none;
    }

    public void ShowRotateControls()
    {
        ShowHUD();

        rightGraphic.sprite = stick;
        leftGraphic.sprite = stick;
    }

    public void ShowGrabControls()
    {
        ShowHUD();

        rightGraphic.sprite = grip;
        leftGraphic.sprite = grip;
    }

    public void ShowTeleportControls()
    {
        ShowHUD();

        if (settings.RightHanded)
        {
            rightGraphic.sprite = one;
            leftGraphic.sprite = none;
        }
        else
        {
            rightGraphic.sprite = none;
            leftGraphic.sprite = one;
        }
    }

    public void ShowKeypadControls()
    {
        ShowHUD();

        if (settings.RightHanded)
        {
            rightGraphic.sprite = none;
            leftGraphic.sprite = one;
        }
        else
        {
            rightGraphic.sprite = one;
            leftGraphic.sprite = none;
        }
    }
}
