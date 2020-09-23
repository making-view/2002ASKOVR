using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AnimateCanvasGroupAlpha : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        canvasGroup.alpha = Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad * 2));
    }
}
