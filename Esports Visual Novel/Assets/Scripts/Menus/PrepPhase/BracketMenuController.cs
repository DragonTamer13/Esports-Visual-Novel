using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls what is shown on the bracket window. Functions are desinged to be called from a Fungus flowchart.
/// </summary>
public class BracketMenuController : Menu
{
    // All the text fields that can show the player team's name.
    [SerializeField] private List<TextMeshProUGUI> playerTeamNameTexts = new List<TextMeshProUGUI>();

    // The background bracket picture.
    [SerializeField] private Image bracketImage;

    private string playerTeamName;

    /// <summary>
    /// Sets up the bracket window for the day.
    /// </summary>
    /// <param name="newBracketImage">Background image to display.</param>
    /// <param name="teamName">The player team name.</param>
    public void DesignBracket(Sprite newBracketImage, string teamName)
    {
        bracketImage.sprite = newBracketImage;
        playerTeamName = teamName;
        playerTeamNameTexts[0].text = playerTeamName;
    }

    /// <summary>
    /// Use from a flowchart to set up which text boxes should display the player team's name.
    /// </summary>
    /// <param name="textBoxIndex"></param>
    public void ShowPlayerNameInTextBox(int textBoxIndex)
    {
        playerTeamNameTexts[textBoxIndex].text = playerTeamName;
    }
}
