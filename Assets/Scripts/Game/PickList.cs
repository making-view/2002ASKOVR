using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct OrderItem
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

//
// Handles the progression of the picking game
// If the user is in picking phase, the game expects to be given the amount of stock picked
// If not in picking phase, it expects to be given the stockcode of the next item on the orderlist
//
public class PickList : MonoBehaviour
{
    public static List<ShelfInfo> shelves = new List<ShelfInfo>();

    [SerializeField] private VoiceCommandLady voiceCommandLady = null;
    [SerializeField] public List<OrderItem> orderItems = null;

    private int currentItem = 0;
    private bool picking = false;
    private string currentStockCode = "000";

    //
    // Starts off the game
    //
    public void Initialize()
    {
        voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
        currentStockCode = shelves.FirstOrDefault(s => s.shelfNo.Equals(orderItems[currentItem].shelfNo)).stockCode;
    }

    //
    // Parses the given string and executs the appropriate action if it matches an expected command
    //
    public void ReceiveCommand(string command)
    {
        if (command.ToLower().Equals("repeat"))
        {
            RepeatCommand();
        }

        if (picking)
        {
            if (currentItem < orderItems.Count)
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

    //
    // Tries to progress to the next item on the orderlist, finishes game if current item was the last one
    //
    private void NextItem()
    {
        currentItem++;
        picking = false;

        if (currentItem >= orderItems.Count)
        {
            voiceCommandLady.PlayEndStateCommand();
            FindObjectOfType<GameManager>().EndGame();
        }
        else
        {
            voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
            currentStockCode = shelves.FirstOrDefault(s => s.shelfNo.Equals(orderItems[currentItem].shelfNo)).stockCode;
        }
    }

    //
    // Plays the voice command for the current step again
    //
    private void RepeatCommand()
    {
        if (picking)
            voiceCommandLady.PlayStockPickCommand(orderItems[currentItem].amount);
        else
            voiceCommandLady.PlayCShelfCommand(orderItems[currentItem].shelfNo);
    }
}
