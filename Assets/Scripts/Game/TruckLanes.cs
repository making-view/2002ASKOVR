using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckLanes : MonoBehaviour
{
    [SerializeField] private Transform[] lanes = null;

    //
    // Finds which lane has the shortest X-axis distance to the given point
    //
    public int FindLaneClosestToPoint(Vector3 point)
    {
        int result = 0;
        var currMin = Mathf.Infinity;

        for (int lane = 0; lane < lanes.Length; ++lane)
        {
            var xDist = Mathf.Abs(lanes[lane].position.x - point.x);

            if (xDist < currMin)
            {
                result = lane;
                currMin = xDist;
            }
        }

        return result;
    }

    public Vector3 GetLanePosition(int lane)
    {
        return lanes[lane].position;
    }
}
