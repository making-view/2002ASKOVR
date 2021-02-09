using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableButton))]
public class DebugPressButtonDelayed : MonoBehaviour
{
    InteractableButton button = null;
    [SerializeField] float delay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            enabled = false;
            return;
        }

        button = GetComponent<InteractableButton>();
        StartCoroutine(pressButtonDelayed());
    }

    IEnumerator pressButtonDelayed()
    {
        yield return new WaitForSeconds(delay);
        button.OnClick.Invoke();
    }
}
