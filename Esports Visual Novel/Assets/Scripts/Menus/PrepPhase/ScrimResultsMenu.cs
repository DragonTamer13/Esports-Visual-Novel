using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;

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

    // Names of Flowchart variables containing the Vector4s with each player's stats.
    private readonly string[] FlowchartStatVariableNames = { "MaedayStats", "HajoonStats", "VelocityStats", "AtroposStats", "BoigaStats" };
    // Names of Flowchart variables containing the strings for player notes today, if they are different than the default notes.
    private readonly string[] FlowchartNotesVariableNames = { "MaedayNotes", "HajoonNotes", "VelocityNotes", "AtroposNotes", "BoigaNotes" };

    [Tooltip("Sprites for indicating that a stat went up.")]
    [SerializeField] private Sprite statUpArrow;
    [Tooltip("Sprites for indicating that a stat went down.")]
    [SerializeField] private Sprite statDownArrow;
    [Tooltip("Sprites for indicating that a stat didn't change.")]
    [SerializeField] private Sprite statsNeutralArrow;
    [Tooltip("Sprites for each possible numerical stat value, in decreasing order.")]
    [SerializeField] private Sprite[] valueImages;
    [Tooltip("Image components for displaying numerical stat values.")]
    [SerializeField] private Image[] values;
    [Tooltip("Text component for displaying notes.")]
    [SerializeField] private TextMeshProUGUI[] notes;

    private CanvasGroup canvasGroup;
    private Flowchart datastoreFlowchart;
    // Image components for indicating if a stat went up or down.
    private List<Image> valueArrows = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject datastoreGameObject = GameObject.Find("DatastoreFlowchart");
        if (datastoreGameObject != null)
        {
            datastoreFlowchart = datastoreGameObject.GetComponent<Flowchart>();
        }

        canvasGroup = GetComponent<CanvasGroup>();
        // Find the arrow image component on Start so we don't have to set it in inspector.
        foreach (Image image in values)
        {
            valueArrows.Add(image.transform.parent.Find("Arrow").GetComponent<Image>());
        }
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
        if (matchDay == PrepPhaseMenuController.MatchDay.Credits)
        {
            return;
        }

        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + ScrimResultsCSVPath))
        {
            Vector4 startOfPrepCharacterStats;
            Vector4 prevCharacterStats;
            string line = "";
            string note = "";
            int linesToSkip = 5 * ((int)matchDay);  // There are 5 rows for each match day.

            // Read stat variables from DatastoreFlowchart
            for (int player = 0; player < FlowchartStatVariableNames.Length; player++)
            {
                // Copy the current stat values to the "StartOfPrep" variable. This is used to keep track of what the stat values were 
                // at the beginning of this prep phase. Necessary because stat values will change during the prep phase, and the player
                // can save and reload while in the middle of the prep phase.
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrep" + FlowchartStatVariableNames[player]).Apply(SetOperator.Assign,
                    datastoreFlowchart.GetVariable<Vector4Variable>(FlowchartStatVariableNames[player]));

                startOfPrepCharacterStats = datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrep" + FlowchartStatVariableNames[player]).Value;
                prevCharacterStats = datastoreFlowchart.GetVariable<Vector4Variable>("Prev" + FlowchartStatVariableNames[player]).Value;

                if (startOfPrepCharacterStats.x < 1 || startOfPrepCharacterStats.y < 1 || startOfPrepCharacterStats.z < 1 || startOfPrepCharacterStats.w < 1)
                {
                    Debug.LogError("A character's stat value is less than 1. Check stat variables in DatastoreFlowchart.");
                    continue;
                }

                // Set each image to be a sprite displaying the player's rating for each stat. Stat values are between 1 and 5, inclusive.
                values[player * 4 + 0].sprite = valueImages[Mathf.RoundToInt(startOfPrepCharacterStats.x) - 1];
                values[player * 4 + 1].sprite = valueImages[Mathf.RoundToInt(startOfPrepCharacterStats.y) - 1];
                values[player * 4 + 2].sprite = valueImages[Mathf.RoundToInt(startOfPrepCharacterStats.z) - 1];
                values[player * 4 + 3].sprite = valueImages[Mathf.RoundToInt(startOfPrepCharacterStats.w) - 1];

                // Set image if stat went up or down
                SetValueArrowImage(valueArrows[player * 4 + 0], Mathf.RoundToInt(startOfPrepCharacterStats.x), Mathf.RoundToInt(prevCharacterStats.x));
                SetValueArrowImage(valueArrows[player * 4 + 1], Mathf.RoundToInt(startOfPrepCharacterStats.y), Mathf.RoundToInt(prevCharacterStats.y));
                SetValueArrowImage(valueArrows[player * 4 + 2], Mathf.RoundToInt(startOfPrepCharacterStats.z), Mathf.RoundToInt(prevCharacterStats.z));
                SetValueArrowImage(valueArrows[player * 4 + 3], Mathf.RoundToInt(startOfPrepCharacterStats.w), Mathf.RoundToInt(prevCharacterStats.w));
            }

            // The scrim results for this day will be 5 contiguous rows in the CSV. Skip all rows that come before the desired day.
            // TODO: Read only notes from CSV. Just get rid of the stats columns.
            for (int skip = 0; skip < linesToSkip; skip++)
            {
                reader.ReadLine();
            }

            for (int notesIndex = 0; notesIndex < 5; notesIndex++)
            {
                line = reader.ReadLine();

                // First, see if there is a notes override string in the DatastoreFlowchart. Otherwise, read the default value from the CSV.
                note = datastoreFlowchart.GetVariable<StringVariable>(FlowchartNotesVariableNames[notesIndex]).Value;
                if (note == "")
                {
                    note = line.Substring(8);
                    // Trim off quotes if the string has them.
                    if (note != "")
                    {
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
                    }
                }

                notes[notesIndex].text = note;
            }
        }
    }

    /// <summary>
    /// Set the image indicating if the corresponding stat value went up or down.
    /// </summary>
    /// <param name="image">Image component to display the arrow image.</param>
    private void SetValueArrowImage(Image image, int curValue, int prevValue)
    {
        if (curValue > prevValue)
        {
            image.sprite = statUpArrow;
        }
        else if (curValue < prevValue)
        {
            image.sprite = statDownArrow;
        }
        else
        {
            image.sprite = statsNeutralArrow;
        }
    }
}
