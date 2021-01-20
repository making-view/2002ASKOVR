using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
    public class BezierSqauencer : MonoBehaviour
    {

        [SerializeField] List<BezierSpline> splines = null;
        [SerializeField] BezierWalkerWithTime walker = null;
        [SerializeField] FollowTruck follower = null;

        private int splineNum = 0;

        private void Start()
        {
            if (splines.Count <= 0)
                Debug.LogError("no splines added to " + name);
            
            walker.spline = splines[0];
            follower.spline = splines[0];
        }

        private void Update()
        {
           if(Input.GetKeyDown(KeyCode.Space))
            {
                if (splineNum == 0 && walker.NormalizedT == 0)
                    StartMovement();
                else
                    NextPath();
            }
        }

        public void StartMovement()
        {
            walker.enabled = true;
            walker.NormalizedT = 0.0f;
        }

        public void StopMovement()
        {
            walker.enabled = false;
        }

        public void NextPath()
        {
            splineNum ++;

            if(splineNum <= splines.Count + 1)
            {
                follower.spline = splines[splineNum];
                walker.spline = splines[splineNum];
                StartMovement();
            }
        }
    }
}
