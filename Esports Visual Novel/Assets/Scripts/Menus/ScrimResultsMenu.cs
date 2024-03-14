using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrimResultsMenu : MonoBehaviour
{
    private readonly string ScrimResultsCSVPath = "/ScrimResults.csv";

    public enum MatchDay
    {
        Tuesday_1,
        Wednesday_2,
        Thursday_3,
        Friday_4_vs_3
    }

    [SerializeField] private Image[] numberBackgrounds;
    [SerializeField] private TextMeshProUGUI[] notes;

    private List<TextMeshProUGUI> numberText = new List<TextMeshProUGUI>();
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        // Each number background has a corresponding text element
        foreach (Image i in numberBackgrounds)
        {
            numberText.Add(i.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }

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
    public void SetupScrimResults(MatchDay matchDay)
    {
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + ScrimResultsCSVPath))
        {
            string line = "";
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
                            numberText[numberCounter].text = "1";
                            numberBackgrounds[numberCounter].color = new Color(0.902f, 0.486f, 0.451f, 1.0f);
                            break;
                        case "2":
                            numberText[numberCounter].text = "2";
                            numberBackgrounds[numberCounter].color = new Color(0.949f, 0.663f, 0.427f, 1.0f);
                            break;
                        case "3":
                            numberText[numberCounter].text = "3";
                            numberBackgrounds[numberCounter].color = new Color(1.0f, 0.839f, 0.400f, 1.0f);
                            break;
                        case "4":
                            numberText[numberCounter].text = "4";
                            numberBackgrounds[numberCounter].color = new Color(0.859f, 0.859f, 0.600f, 1.0f);
                            break;
                        case "5":
                            numberText[numberCounter].text = "5";
                            numberBackgrounds[numberCounter].color = new Color(0.718f, 0.882f, 0.804f, 1.0f);
                            break;
                        default:
                            numberBackgrounds[numberCounter].color = Color.white;
                            break;
                    }
                    numberCounter++;
                }
                notes[notesCounter].text = line.Substring(8);
                // Trim off quotes if the string has them.
                if (notes[notesCounter].text[0] == '"')
                {
                    notes[notesCounter].text = notes[notesCounter].text.Substring(1, notes[notesCounter].text.Length - 2);
                }

                notesCounter++;
            }
        }
    }
}
