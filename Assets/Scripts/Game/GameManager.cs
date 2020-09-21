using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Report
{
    public List<string> reasons;
    public int score;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] PickList pickList = null;
    [SerializeField] Truck truck = null;
    [SerializeField] CarryingArea carryingArea = null;

    bool finishedPicking = false;
    float timer = 0.0f;

    public void Start()
    {
        //TODO find better way to start game than just automatically starting immediately
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(Timer());
        pickList.Initialize();
    }

    public void EndGame()
    {
        finishedPicking = true;
        pickList.gameObject.SetActive(false); // Deactivated because game might end before PickList is complete
        var report = GenerateReport();

        // Display report to user
    }

    //
    // Generates a report containing the users score and a list of reasons for the given score
    //
    private Report GenerateReport()
    {
        var report = new Report();
        report.reasons = new List<string>();
        report.reasons.Add("You picked things lol");
        report.score = (int)(500 - timer);

        return report;
    }

    //
    // Increases timer while picking is still underway
    //
    private IEnumerator Timer()
    {
        while (!finishedPicking)
        {
            timer += Time.deltaTime;

            yield return null;
        }
    }
}
