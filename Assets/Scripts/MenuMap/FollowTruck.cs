using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTruck : MonoBehaviour
{
    [SerializeField] Transform frontMount = null;
    [SerializeField] Transform backMount = null;
    [SerializeField] Transform middlePoint = null;

    [SerializeField] public BezierSolution.BezierSpline spline = null;
    [SerializeField] BezierSolution.BezierWalkerWithTime cabin = null;

    // Update is called once per frame

    //private void Start()
    //{
    //    var walker = Instantiate<BezierSolution.BezierWalkerWithTime>(cabin);
    //    walker.lookAt(frontMount);
    //}

    void Update()
    {

        float normalizedT = 0;

        spline.FindNearestPointTo(backMount.position, out normalizedT, 1000);
        Vector3 tangent = spline.GetTangent(normalizedT);

        Vector3 distanceToMove = frontMount.position - backMount.position;
        this.transform.position += distanceToMove;

        if (cabin.MovingForward)
            this.transform.LookAt(this.transform.position + tangent / 100);
        else
            this.transform.LookAt(this.transform.position - tangent / 100);
    }
}
