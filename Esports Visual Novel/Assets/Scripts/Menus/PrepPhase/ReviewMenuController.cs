﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;   

/// <summary>
/// Main script for menu in the Prep Phase where the Coach can talk with the players.
/// </summary>
public class ReviewMenuController : Menu
{
    [SerializeField] private Button boigaButton;
    [SerializeField] private Text reviewsRemainingText;

    // Flowchart containing the data and dialogue nodes for this day.
    private Flowchart dayFlowchart;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void Show()
    {
        base.Show();
        UpdateButtons();
    }

    /// <summary>
    /// Changes if review buttons are enabled/disabled for each character by looking at flowchart variables.
    /// </summary>
    public void UpdateButtons()
    {
        if (dayFlowchart == null)
        {
            // NOTE: To find the flowchart, we assume that the day flowchart isn't named "DatastoreFlowchart"
            // and contains a variable called "CoachingsLeft". All flowcharts with player coaching sequences
            // should have a collection of variables that are all named the same. This menu shouldn't be accessible
            // when the player can't coach any more players.
            foreach (Flowchart flowchart in FindObjectsOfType<Flowchart>())
            {
                if (flowchart.gameObject.name != "DatastoreFlowchart" && flowchart.GetIntegerVariable("CoachingsLeft") != 0)
                {
                    dayFlowchart = flowchart;
                }
            }
            if (dayFlowchart == null)
            {
                Debug.LogError("Attempting to update the ReviewMenuController without the correct flowchart variables in the scene.");
                return;
            }
        }

        if (dayFlowchart.GetBooleanVariable("TalkedToBoiga"))
        {
            boigaButton.enabled = false;
            boigaButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }

        reviewsRemainingText.text = dayFlowchart.GetStringVariable("CoachingsLeft");
    }
}
