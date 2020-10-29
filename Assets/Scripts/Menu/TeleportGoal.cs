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

    private void OnTriggerEnter(Collider other)
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
         
        var didAction = tutorial.DoAction(Tutorial.ToDo.Teleport);

        if (didAction)
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
