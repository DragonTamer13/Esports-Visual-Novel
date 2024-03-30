using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.UI;

public class PlayerNameMenu : MonoBehaviour
{
    [SerializeField] private Flowchart datastoreFlowchart;
    [SerializeField] private Text nameInputField;
    [SerializeField] private Text teamNameInputField;

    /// <summary>
    /// Set value for player name in the datastore flowchart.
    /// </summary>
    public void SetPlayerName()
    {
        datastoreFlowchart.SetStringVariable("PlayerName", "Coach " + nameInputField.text);
    }

    /// <summary>
    /// Set value for team name in the datastore flowchart.
    /// </summary>
    public void SetTeamName()
    {
        datastoreFlowchart.SetStringVariable("PlayerTeam", teamNameInputField.text);
    }
}
