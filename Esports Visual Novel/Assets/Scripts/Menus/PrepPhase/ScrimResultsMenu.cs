using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrimResultsMenu : MonoBehaviour
{
    private readonly string ScrimResultsCSVPath = "/ScrimResults.csv";
    // Contains the color to highlight each Pantheon character's name with.
    private readonly Dictionary<string, string> characterNamesToHighlight = new Dictionary<string, string>() 
    { 
        {"Artemis", "60DE65"},
        {"Athena", "3191D8"},
        {"Hades", "8650BB"},
        {"Helios", "C10000"},
        {"Hermes", "FACD6B"},
        {"Persephone", "C200C6"},
        {"Ra", "E86100"},
    };

    [Tooltip("Sprites for each possible numerical stat value, in decreasing order.")]
    [SerializeField] private Sprite[] valueImages;
    [Tooltip("Image components for displaying numerical stat values.")]
    [SerializeField] private Image[] values;
    [Tooltip("Text component for displaying notes.")]
    [SerializeField] private TextMeshProUGUI[] notes;

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    /// <summary>
    /// Show the scrim results menu.
    /// </summary>
    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide the scrim results menu.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Set up the stats and comments for the players for a desired match day.
    /// </summary>
    /// <param name="matchDay">The day to display scrim results for.</param>
    public void SetupScrimResults(PrepPhaseMenuController.MatchDay matchDay)
    {
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + ScrimResultsCSVPath))
        {
            string line = "";
            string note = "";
            int linesToSkip = 5 * ((int)matchDay);  // There are 5 rows for each match day.
            int numberCounter = 0;
            int notesCounter = 0;

            // The scrim results for this day will be 5 contiguous rows in the CSV. Skip all rows that come before the desired day.
            for (int skip = 0; skip < linesToSkip; skip++)
            {
                reader.ReadLine();
            }

            for (int dataLine = 0; dataLine < 5; dataLine++)
            {
                line = reader.ReadLine();

                for (int i = 0; i < 4; i++)
                {
                    switch (line.Substring(i * 2, 1))
                    {
                        case "1":
                            values[numberCounter].sprite = valueImages[0];
                            break;
                        case "2":
                            values[numberCounter].sprite = valueImages[1];
                            break;
                        case "3":
                            values[numberCounter].sprite = valueImages[2];
                            break;
                        case "4":
                            values[numberCounter].sprite = valueImages[3];
                            break;
                        case "5":
                            values[numberCounter].sprite = valueImages[4];
                            break;
                        default:
                            values[numberCounter].color = Color.white;
                            break;
                    }
                    numberCounter++;
                }

                note = line.Substring(8);
                // Trim off quotes if the string has them.
                if (note[0] == '"')
                {
                    note = note.Substring(1, note.Length - 2);
                }
                // Highlight all character names in the note.
                foreach (KeyValuePair<string, string> pair in characterNamesToHighlight)
                {
                    // TODO: Pray that none of the notes are prefixed with a character's name.
                    note = note.Replace(pair.Key, "<color=#" + pair.Value + ">" + pair.Key + "</color>");
                }

                notes[notesCounter].text = note;
                notesCounter++;
            }
        }
    }
}
