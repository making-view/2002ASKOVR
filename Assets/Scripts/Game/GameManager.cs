using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] Text debugText = null;

    bool finishedPicking = false;
    float timer = 0.0f;

    public void StartGame()
    {
        if (finishedPicking || timer > 0.0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(Timer());
            pickList.Initialize();
        }
    }

    public void EndGame()
    {
        finishedPicking = true;
        pickList.gameObject.SetActive(false); // Deactivated because game might end before PickList is complete
        var report = GenerateReport();

        if (debugText != null)
        {
            PrintReport(report);
        }

        FindObjectOfType<VoiceCommandLady>().Hehehoho();
    }

    private void PrintReport(Report report)
    {
        debugText.text = "";

        foreach (var reason in report.reasons)
        {
            debugText.text += reason;
            debugText.text += System.Environment.NewLine;
        }

        debugText.text += "Total score: " + report.score.ToString();
    }

    //
    // Generates a report containing the users score and a list of reasons for the given score
    //
    private Report GenerateReport()
    {
        var report = new Report();
        report.reasons = new List<string>();

        if (truck.StockFellOff)
        {
            report.reasons.Add("Diskvalifisert: Varer falt av.");
            report.score = 0;
        }
        else
        {
            var timeScore = Mathf.Clamp((int)(500 - timer), 0, 500);
            report.reasons.Add("Tidsbonus: " + timeScore);
            report.score += timeScore;

            var correctStock = new List<Stock>();
            var missingPicks = 0;
            var superfluousPicks = 0;
            var shouldPick = 0;

            foreach (var orderItem in pickList.orderItems)
            {
                var picked = carryingArea.CarriedStock.Where(s => s.ShelfNumber == orderItem.shelfNo).ToList();
                correctStock.AddRange(picked.GetRange(0, Mathf.Clamp(orderItem.amount, 0, picked.Count)));
                shouldPick += orderItem.amount;

                missingPicks += (int)Mathf.Abs(Mathf.Clamp(picked.Count - orderItem.amount, -Mathf.Infinity, 0));
                superfluousPicks += (int)Mathf.Clamp(picked.Count - orderItem.amount, 0, Mathf.Infinity);
            }

            var pickedScore = correctStock.Count * 10;
            report.reasons.Add("Riktig plukk x" + correctStock.Count + ": " + pickedScore);
            report.score += pickedScore;

            var extraNonOrderStock = carryingArea.CarriedStock.ToList().Count - (correctStock.Count + superfluousPicks);
            var totalIncorrectPicks = missingPicks + superfluousPicks + extraNonOrderStock;
            var minusScore = Mathf.Clamp(totalIncorrectPicks * -10, -pickedScore, 0);

            report.reasons.Add("Feil plukk x" + totalIncorrectPicks + ": " + minusScore);

            var totStickyWares = 0;
            var stabilityScore = 0;

            foreach (var stock in carryingArea.CarriedStock)
            {
                var currCount = stock.GetStockBelow().Count;
                totStickyWares += currCount;
                stabilityScore += currCount * 10;
            }

            if (totStickyWares > 0)
            {
                report.reasons.Add("Varebinding x" + totStickyWares + ": " + stabilityScore);
                report.score += stabilityScore;
            }

            var driveScore = 250 - Mathf.Clamp((truck.UnsafeMovements * -50), -250, 0);
            report.reasons.Add("Uaktsom kjøring x" + truck.UnsafeMovements + ": " + driveScore);
            report.score += driveScore;

            var rightFactor = (float)correctStock.Count / shouldPick;
            report.reasons.Add("Andel riktig plukk: " + (int)(rightFactor * 100) + "%");

            report.score = (int)(rightFactor * report.score);
        }

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
