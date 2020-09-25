using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

struct Report
{
    public List<ReportEntry> entries;
    public float correctPickFactor;
    public string correctPickString;
    public int imprecision;
}

struct ReportEntry
{
    public string reason;
    public int score;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] PickList pickList = null;
    [SerializeField] Truck truck = null;
    [SerializeField] CarryingArea carryingArea = null;
    [SerializeField] Measurer measurer = null;
    [SerializeField] Transform palletDestination = null;
    [SerializeField] Transform playerDestination = null;
    [SerializeField] OVRCameraRig cameraRig = null;
    [SerializeField] Camera playerHead = null;
    [SerializeField] ReportManager reportManager = null;
    [SerializeField] ReportRow reportRowTemplate = null;
    [SerializeField] int maxImprecision = 25;

    bool finishedPicking = false;
    float timer = 0.0f;

    public bool IsGameRunning
    {
        get
        {
            return timer > 0.0f && !finishedPicking;
        }
    }

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
        truck.DisableTruck();

        GoToReport();

        StartCoroutine(GenerateReport());
    }

    //
    // Moves all stock and player to report area
    //
    private void GoToReport()
    {
        truck.transform.rotation = Quaternion.Euler(0, 90, 0);
        var moveOffset = carryingArea.transform.position - truck.transform.position;
        var newTruckPos = palletDestination.position - moveOffset;
        truck.transform.position = new Vector3(newTruckPos.x, truck.transform.position.y, newTruckPos.z);

        var offset = playerHead.transform.position - cameraRig.transform.position;
        Vector3 xzPlaneOffset = new Vector3(offset.x, 0, offset.z);

        var rotOffset = playerHead.transform.rotation.eulerAngles.y - cameraRig.transform.rotation.eulerAngles.y;

        cameraRig.transform.position = playerDestination.position - xzPlaneOffset;
        cameraRig.transform.rotation = Quaternion.Euler(playerDestination.rotation.eulerAngles - new Vector3(0, rotOffset, 0));
    }

    //
    // Generates a report containing the users score and a list of reasons for the given score
    //
    private IEnumerator GenerateReport()
    {
        var report = new Report();
        report.entries = new List<ReportEntry>();
        reportManager.gameObject.SetActive(true);

        if (!truck.StockFellOff)
        {
            var timeScore = Mathf.Clamp((int)((1000 - timer) / 2), 0, 500);
            report.entries.Add(new ReportEntry() { reason = "Tidsbonus: ", score = timeScore });

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
            report.entries.Add(new ReportEntry() { reason = "Riktig plukk x" + correctStock.Count + ": ", score = pickedScore });
            report.correctPickFactor = (float)correctStock.Count / shouldPick;
            report.correctPickString = correctStock.Count + "/" + shouldPick;

            var extraNonOrderStock = carryingArea.CarriedStock.ToList().Count - (correctStock.Count + superfluousPicks);
            var totalIncorrectPicks = missingPicks + superfluousPicks + extraNonOrderStock;
            var minusScore = Mathf.Clamp(totalIncorrectPicks * -10, -pickedScore, 0);

            report.entries.Add(new ReportEntry() { reason = "Feil plukk x" + totalIncorrectPicks + ": ", score = minusScore });

            var totStickyWares = 0;
            var stabilityScore = 0;

            foreach (var stock in carryingArea.CarriedStock)
            {
                var currCount = stock.GetStockBelow().Count;
                totStickyWares += currCount;
                stabilityScore += currCount * 10;
            }

            if (totStickyWares > 0)
                report.entries.Add(new ReportEntry() { reason = "Varebinding x" + totStickyWares + ": ", score = stabilityScore });

            var driveScore = 250 * (1 - (truck.UnsafeMovement / truck.TotalMovement));
            var safeMovement = truck.TotalMovement - truck.UnsafeMovement;
            report.entries.Add(new ReportEntry() { reason = "Trygg kjøring " + safeMovement.ToString("0.0") 
                + "m/" + truck.TotalMovement.ToString("0.0") + "m: ",
                score = (int)driveScore });

            yield return StartCoroutine(measurer.MeasureAll());

            var totalImprecision = 0;
            foreach (var imprecision in measurer.imprecisions)
            {
                totalImprecision += imprecision;
            }

            var precisionScore = Mathf.Clamp(maxImprecision - totalImprecision, 0, maxImprecision) * (500 / maxImprecision);
            report.entries.Add(new ReportEntry() { reason = "Unøyaktighet " + totalImprecision + "cm: ", score = precisionScore });
            report.imprecision = totalImprecision;
        }

        yield return StartCoroutine(VisualizeReport(report));
    }

    //
    // Shows the report and animates values and grades over time for visual flair
    //
    private IEnumerator VisualizeReport(Report report)
    {
        if (truck.StockFellOff)
        {
            reportManager.messageText.text = "Usikrede varer falt av pallen din";
            reportManager.messageText.text += Environment.NewLine;
            reportManager.messageText.text += Environment.NewLine;
            reportManager.messageText.text += "Diskvalifisert";
            reportManager.messageText.gameObject.SetActive(true);
        }
        else if (report.imprecision > maxImprecision)
        {
            reportManager.messageText.text = "Varene dine er ikke stablet presist nok på pallen";
            reportManager.messageText.text += Environment.NewLine;
            reportManager.messageText.text += Environment.NewLine;
            reportManager.messageText.text += "Diskvalifisert";
            reportManager.messageText.gameObject.SetActive(true);
        }
        else
        {
            reportManager.messageText.gameObject.SetActive(false);

            foreach (ReportEntry entry in report.entries)
            {
                var row = Instantiate(reportRowTemplate);
                row.transform.SetParent(reportManager.Entries.gameObject.transform, false);

                row.reasonText.text = entry.reason;
                row.scoreText.text = entry.score.ToString();

                row.gameObject.SetActive(true);

                yield return null;
            }

            var totalScore = 0;
            foreach (var entry in report.entries)
            {
                totalScore += entry.score;
            }

            reportManager.totalScoreText.text = totalScore.ToString();

            reportManager.pickFactorText.gameObject.SetActive(true);
            reportManager.pickFactorText.text = report.correctPickString;
            reportManager.pickFactorNum.text = "x" + report.correctPickFactor.ToString("0.00");

            reportManager.totalScoreText.gameObject.SetActive(true);
            reportManager.totalGradeImage.gameObject.SetActive(true);
        }
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
