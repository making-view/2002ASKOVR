using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the pitch and playstate of an audio source based on spline walker movement
/// </summary>
namespace BezierSolution
{
    //[RequireComponent(typeof(BezierSolution.BezierWalker))]
    //[RequireComponent(typeof(AudioSource))]
    public class BezierSoundController : MonoBehaviour
    {
        private BezierWalker walker = null;
        private AudioSource audioSource = null;

        private Vector3 lastPos = Vector3.zero;
        private float speed = 0.0f;
        private float startingPitch;

        private bool wasPlaying = false;

        [SerializeField]  float speedModifier = 500.0f;
        [Tooltip("displays speed after speed modifier. Speed should be close to 1 when going fast")]
        [SerializeField] bool debugSpeed = false;
        [Range(0.0f, 2.0f)] [SerializeField] float pitchInfluence = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            walker = GetComponent<BezierWalker>();
            audioSource = GetComponent<AudioSource>();

            lastPos = transform.position;
            startingPitch = audioSource.pitch;
        }

        // Could make this async and more effective
        void Update()
        {
            if (walker.NormalizedT <= 0.0 || walker.NormalizedT >= 1.0f)
            {
                audioSource.Stop();
            }
            else if (!audioSource.isPlaying)
                audioSource.Play();
            else
            {
                speed = (transform.position - lastPos).magnitude;

                speed = Mathf.Clamp(speed * speedModifier, 0.0f, 1.0f);

                if(debugSpeed)
                    Debug.Log("speed: " + speed);

                lastPos = transform.position;

                //TODO, control volume maybe?
                //audioSource.volume = speed;
                audioSource.pitch = startingPitch + (speed - 0.5f) *pitchInfluence;
            }
        }
    }
}