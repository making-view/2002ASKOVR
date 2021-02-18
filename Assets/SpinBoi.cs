using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBoi : MonoBehaviour
{
    [SerializeField] bool x, y, z;
    [SerializeField] float speed = 10.0f;
    [SerializeField] float spinUpTime = 5.0f;

    private float currentSpeed = 0.0f;

    private Vector3 xVec = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 yVec = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 zVec = new Vector3(0.0f, 0.0f, 1.0f);

    public void startSpin()
    {
        StartCoroutine(SpinUp());
    }

    private IEnumerator SpinUp()
    {
        yield return new WaitForSeconds(1.0f);

        float timeElapsed = 0.0f;
        currentSpeed = 0.0f;
        StartCoroutine(Spin());

        while(timeElapsed < spinUpTime)
        {
            currentSpeed = Mathf.SmoothStep(0.0f, speed, timeElapsed / spinUpTime);
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        currentSpeed = speed;
    }

    private IEnumerator Spin()
    {
        while (true)
        {
            float increase = Time.deltaTime * currentSpeed;

            if (x)
                transform.Rotate(xVec, increase);
            if (y)
                transform.Rotate(yVec, increase);
            if (z)
                transform.Rotate(zVec, increase);

            yield return null;
        }
    }
}
