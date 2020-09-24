using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Measurer : MonoBehaviour
{
    public int[] imprecisions;

    private List<MeasureWall> walls;

    void Awake()
    {
        walls = GetComponentsInChildren<MeasureWall>().ToList();
        imprecisions = new int[walls.Count];

        foreach(var wall in walls)
        {
            wall.gameObject.SetActive(false);
        }
    }

    public IEnumerator MeasureAll()
    {

        foreach (var wall in walls)
        {
            wall.gameObject.SetActive(true);
        }

        for (int i = 0; i < walls.Count; ++i)
        {
            yield return StartCoroutine(walls[i].Measure());
            imprecisions[i] = walls[i].imprecision;
        }
    }
}
