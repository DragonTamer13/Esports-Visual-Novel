using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.UI;

public class PlayerNameMenu : MonoBehaviour
{
    [SerializeField] private Flowchart datastoreFlowchart;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField teamNameInputField;
    [SerializeField] private Button continueButton;

    /// <summary>
    /// Set value for player name in the datastore flowchart.
    /// </summary>
    public void SetPlayerName()
    {
        datastoreFlowchart.SetStringVariable("PlayerName", nameInputField.text);
        continueButton.interactable = (nameInputField.text != "" && teamNameInputField.text != "");
    }

    /// <summary>
    /// Set value for team name in the datastore flowchart.
    /// </summary>
    public void SetTeamName()
    {
        datastoreFlowchart.SetStringVariable("PlayerTeam", teamNameInputField.text);
        continueButton.interactable = (nameInputField.text != "" && teamNameInputField.text != "");
    }
}
