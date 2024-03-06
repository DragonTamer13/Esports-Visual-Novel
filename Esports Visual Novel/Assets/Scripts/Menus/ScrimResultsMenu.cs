using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrimResultsMenu : MonoBehaviour
{
    private readonly string ScrimResultsCSVPath = "/Resources/ScrimResultsTest.csv";

    public enum MatchDay
    {
        Day1,
        Day2
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
        // TODO: Get the path based on the day
        using (StreamReader reader = File.OpenText(Application.dataPath + ScrimResultsCSVPath))
        {
            string line = reader.ReadLine();
            int numberCounter = 0;
            int notesCounter = 0;

            while (line != null)
            {
                string[] parsedLine = line.Split(',');
                for (int i = 0; i < parsedLine.Length; i++)
                {
                    if (i < parsedLine.Length-1)
                    {
                        numberText[numberCounter].text = parsedLine[i];
                        switch (parsedLine[i])
                        {
                            case "1":
                                numberBackgrounds[numberCounter].color = new Color(1.0f, 0.227f, 0.227f, 1.0f);
                                break;
                            case "2":
                                numberBackgrounds[numberCounter].color = new Color(1.0f, 0.631f, 0.333f, 1.0f);
                                break;
                            case "3":
                                numberBackgrounds[numberCounter].color = new Color(1.0f, 1.0f, 0.376f, 1.0f);
                                break;
                            case "4":
                                numberBackgrounds[numberCounter].color = new Color(0.765f, 1.0f, 0.459f, 1.0f);
                                break;
                            case "5":
                                numberBackgrounds[numberCounter].color = new Color(0.353f, 1.0f, 0.353f, 1.0f);
                                break;
                            default:
                                numberBackgrounds[numberCounter].color = Color.white;
                                break;
                        }
                        numberCounter++;
                    }
                    else
                    {
                        notes[notesCounter].text = parsedLine[i];
                        notesCounter++;
                    }
                }
                line = reader.ReadLine();
            }
        }
    }
}
