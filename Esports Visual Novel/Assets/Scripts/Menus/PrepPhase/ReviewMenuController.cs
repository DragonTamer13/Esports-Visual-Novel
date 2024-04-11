using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;   

/// <summary>
/// Main script for menu in the Prep Phase where the Coach can talk with the players.
/// </summary>
public class ReviewMenuController : Menu
{
    [SerializeField] private Button atroposButton;
    [SerializeField] private Button boigaButton;
    [SerializeField] private Button hajoonButton;
    [SerializeField] private Button maedayButton;
    [SerializeField] private Button velocityButton;
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
            // should have a collection of variables that are all named the same.
            foreach (Flowchart flowchart in FindObjectsOfType<Flowchart>())
            {
                if (flowchart.gameObject.name != "DatastoreFlowchart" && flowchart.GetVariable("CoachingsLeft") != null)
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

        if (dayFlowchart.GetBooleanVariable("TalkedToAtropos"))
        {
            atroposButton.enabled = false;
            atroposButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }
        if (dayFlowchart.GetBooleanVariable("TalkedToBoiga"))
        {
            boigaButton.enabled = false;
            boigaButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }
        if (dayFlowchart.GetBooleanVariable("TalkedToHajoon"))
        {
            hajoonButton.enabled = false;
            hajoonButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }
        if (dayFlowchart.GetBooleanVariable("TalkedToMaeday"))
        {
            maedayButton.enabled = false;
            maedayButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }
        if (dayFlowchart.GetBooleanVariable("TalkedToVelocity"))
        {
            velocityButton.enabled = false;
            velocityButton.transform.Find("DisabledCover").gameObject.SetActive(true);
        }

        reviewsRemainingText.text = dayFlowchart.GetIntegerVariable("CoachingsLeft").ToString();
    }
}
