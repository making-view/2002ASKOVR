using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGoal : MonoBehaviour
{
    private Tutorial tutorial = null;
    private AudioSource audioSource = null;

    private bool isDying = false;

    private void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
        audioSource = GetComponent<AudioSource>();
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
            audioSource.Play();

            while (audioSource.isPlaying)
                yield return null;

            Destroy(gameObject);
        }
        else
            isDying = false;
    }
}
