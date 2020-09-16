using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct OrderItem
{
    public int amount;
    public int shelfNo;
}

[Serializable]
public struct ShelfInfo
{
    public int shelfNo;
    public int amount;
    public string stockCode;
}

public class PickList : MonoBehaviour
{
    public static List<ShelfInfo> shelves = new List<ShelfInfo>();

    [SerializeField] private VoiceCommandLady voiceCommandLady = null;
    [SerializeField] private List<OrderItem> orderItems = null;

    private int currentItem = 0;
    private bool picking = false;
    private string currentStockCode = "000";

    public void Initialize()
    {
        voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
        currentStockCode = shelves.FirstOrDefault(s => s.shelfNo.Equals(orderItems[currentItem].shelfNo)).stockCode;
    }

    public void ReceiveCommand(string command)
    {
        if (command.ToLower().Equals("repeat"))
        {
            RepeatCommand();
        }

        if (picking)
        {
            if (command.Equals(orderItems[currentItem].amount.ToString()))
            {
                NextItem();
            }
            else
            {
                RepeatCommand();
            }
        }
        else
        {
            if (command.Equals(currentStockCode))
            {
                picking = true;
                voiceCommandLady.PlayStockPickCommand(orderItems[currentItem].amount);
            }
            else
            {
                RepeatCommand();
            }
        }
    }

    private void NextItem()
    {
        if (currentItem < orderItems.Count)
        {
            currentItem++;
            picking = false;
            voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
            currentStockCode = shelves.FirstOrDefault(s => s.shelfNo.Equals(orderItems[currentItem].shelfNo)).stockCode;
        }
        else
        {
            Debug.Log("PICKY DONE");
            voiceCommandLady.Hehehoho();
        }
    }

    private void RepeatCommand()
    {
        if (picking)
            voiceCommandLady.PlayStockPickCommand(orderItems[currentItem].amount);
        else
            voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
    }
}
