﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceCommandLady : MonoBehaviour
{
    [SerializeField] private List<AudioClip> cShelfNumbers = null;
    [SerializeField] private List<AudioClip> stockPickAmounts = null;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCShelfCommand(int shelfNo)
    {
        Debug.Log("Received shelf command: " + shelfNo);

        if (shelfNo > 0 && shelfNo <= cShelfNumbers.Count)
        {
            audioSource.Stop();
            audioSource.clip = cShelfNumbers[shelfNo - 1];
            audioSource.Play();
        }
    }

    public void PlayStockPickCommand(int numberOfStock)
    {
        Debug.Log("Received pick command: " + numberOfStock);

        if (numberOfStock > 0 && numberOfStock <= stockPickAmounts.Count)
        {
            audioSource.Stop();
            audioSource.clip = stockPickAmounts[numberOfStock - 1];
            audioSource.Play();
        }
    }
}
