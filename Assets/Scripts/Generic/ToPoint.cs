using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ToPoint : MonoBehaviour
{
    [SerializeField] GameObject cameraRig = null;


    [SerializeField] bool testOnStart = false;
    [SerializeField] public bool shouldFade = true;
    [Tooltip("Total time for fade-transition to happen")]
    [SerializeField] public float moveTimer = 1.5f;
    [SerializeField] bool parentToNewPosition = false;

    //tells us if the target rig to move is just a camera or the parent of a VR rig (rotation offset difference in calculations)
    private bool isRig = true;

    // Start is called before the first frame update
    void Start()
    {
        CheckforRequiredComponents();

        if (testOnStart)
            StartTransition();
    }

    //looks for needed scripts in children and variables in script
    private void CheckforRequiredComponents()
    {
        //Get OVRPlayerController or camera
        if(cameraRig == null)
            cameraRig = FindObjectOfType<OVRPlayerController>().gameObject;

        if (cameraRig == null || !cameraRig.activeInHierarchy)
        {
            Debug.Log("Vr rig not active, moving parent of cam instead");
            cameraRig = FindObjectOfType<Camera>().gameObject.transform.parent.gameObject;
            isRig = false;
        }

        //default to no fading if no fading element found
        if (cameraRig.GetComponentInChildren<OVRScreenFade>() == null)
            shouldFade = false;
    }

    public void StartTransition()
    {
        StartCoroutine(GoToPoint());
    }

    IEnumerator GoToPoint()
    {
        OVRScreenFade fade = null;


        if (shouldFade)
        {
            //divide fade and movement into 3 chunks (fade-in, wait, fade-out)
            fade = cameraRig.GetComponentInChildren<OVRScreenFade>();
            fade.fadeTime = moveTimer / 3;
            fade.FadeOut();

            //fade out to black
            yield return new WaitForSeconds(fade.fadeTime * 2);
        }
        //start fading and disable controller if there is one

        //todo separate calculation for forward difference VR vs Camera

        //move rig to point w rotation
        var camPos = cameraRig.GetComponentInChildren<Camera>().transform.position;
        var offset = new Vector3(camPos.x - cameraRig.transform.position.x, 0, camPos.z - cameraRig.transform.position.z);

        cameraRig.transform.position = transform.position - offset;
        cameraRig.transform.rotation = transform.rotation;

        if(parentToNewPosition)
            cameraRig.transform.parent = transform;

        //fade in and wait
        if (shouldFade)
        {
            fade.FadeIn();
            yield return new WaitForSeconds(fade.fadeTime);  
        }
    }
}