using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHUDHandler : MonoBehaviour
{
    [SerializeField] private Image rightGraphic = null;
    [SerializeField] private Image leftGraphic = null;

    [Space]
    [SerializeField] private Sprite r_neutral = null;
    [SerializeField] private Sprite r_grip = null;
    [SerializeField] private Sprite r_one = null;
    [SerializeField] private Sprite r_stick = null;

    [Space]
    [SerializeField] private Sprite l_neutral = null;
    [SerializeField] private Sprite l_grip = null;
    [SerializeField] private Sprite l_one = null;
    [SerializeField] private Sprite l_stick = null;


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

        rightGraphic.sprite = r_neutral;
        leftGraphic.sprite = l_neutral;
    }

    public void ShowRotateControls()
    {
        ShowHUD();

        rightGraphic.sprite = r_stick;
        leftGraphic.sprite = l_stick;
    }

    public void ShowGrabControls()
    {
        ShowHUD();

        rightGraphic.sprite = r_grip;
        leftGraphic.sprite = l_grip;
    }

    public void ShowTeleportControls()
    {
        ShowHUD();

        if (settings.RightHanded)
        {
            rightGraphic.sprite = r_one;
            leftGraphic.sprite = l_neutral;
        }
        else
        {
            rightGraphic.sprite = r_neutral;
            leftGraphic.sprite = l_one;
        }
    }

    public void ShowKeypadControls()
    {
        ShowHUD();

        if (settings.RightHanded)
        {
            rightGraphic.sprite = r_neutral;
            leftGraphic.sprite = l_one;
        }
        else
        {
            rightGraphic.sprite = r_one;
            leftGraphic.sprite = l_neutral;
        }
    }
}
