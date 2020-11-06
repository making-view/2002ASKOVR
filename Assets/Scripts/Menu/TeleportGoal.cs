using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGoal : MonoBehaviour
{
    private Tutorial tutorial = null;

    private bool isDying = false;

    private void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isDying && tutorial && tutorial.IsTutorialOngoing)
        {
            if (other.tag.Equals("MainCamera"))
            {
                StartCoroutine(KillSelf());
            }
        }
    }

    private IEnumerator KillSelf()
    {
        isDying = true;
         
        var didTask = tutorial.DoTask(Tutorial.Task.Teleport);

        if (didTask)
        {
            yield return null;

            gameObject.SetActive(false);
        }
        else
            isDying = false;
    }
}
