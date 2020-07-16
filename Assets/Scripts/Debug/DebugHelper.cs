using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHelper : MonoBehaviour
{
    public static DebugHelper Instance { get; private set; }

    private Text text;
    private int prevNum = 0;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        text = GetComponentInChildren<Text>();
    }

    public void Log(string newText)
    {
        text.text = newText;
    }

    public void LogHighestNum(string label, int num)
    {
        if (num > prevNum)
        {
            text.text = label + num;
            prevNum = num;
        }
    }

    public void LogHighestNumAtrophy(string label, int num, float atrophyPercent)
    {
        if (num < prevNum)
            num -= (int)(num * atrophyPercent * Time.deltaTime);

        text.text = label + num;
        prevNum = num;
    }
}
