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

    [Tooltip("Sprites for each possible numerical stat value, in decreasing order.")]
    [SerializeField] private Sprite[] valueImages;
    [Tooltip("Image components for displaying numerical stat values.")]
    [SerializeField] private Image[] values;
    [Tooltip("Text component for displaying notes.")]
    [SerializeField] private TextMeshProUGUI[] notes;

    private CanvasGroup canvasGroup;
    private Flowchart datastoreFlowchart;

    // Start is called before the first frame update
    void Start()
    {
        datastoreFlowchart = GameObject.Find("DatastoreFlowchart").GetComponent<Flowchart>();
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
        if (matchDay == PrepPhaseMenuController.MatchDay.Credits)
        {
            return;
        }

        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + ScrimResultsCSVPath))
        {
            Vector4 characterStats;
            string line = "";
            string note = "";
            int linesToSkip = 5 * ((int)matchDay);  // There are 5 rows for each match day.
            int notesCounter = 0;

            // Read stat variables from DatastoreFlowchart
            for (int player = 0; player < FlowchartStatVariableNames.Length; player++)
            {
                characterStats = datastoreFlowchart.GetVariable<Vector4Variable>(FlowchartStatVariableNames[player]).Value;

                if (characterStats.x < 1 || characterStats.y < 1 || characterStats.z < 1 || characterStats.w < 1)
                {
                    Debug.LogError("A character's stat value is less than 1. Check stat variables in DatastoreFlowchart.");
                    continue;
                }

                // Set each image to be a sprite displaying the player's rating for each stat. Stat values are between 1 and 5, inclusive.
                values[player * 4 + 0].sprite = valueImages[Mathf.RoundToInt(characterStats.x) - 1];
                values[player * 4 + 1].sprite = valueImages[Mathf.RoundToInt(characterStats.y) - 1];
                values[player * 4 + 2].sprite = valueImages[Mathf.RoundToInt(characterStats.z) - 1];
                values[player * 4 + 3].sprite = valueImages[Mathf.RoundToInt(characterStats.w) - 1];
            }

            // The scrim results for this day will be 5 contiguous rows in the CSV. Skip all rows that come before the desired day.
            // TODO: Read only notes from CSV. Just get rid of the stats columns.
            for (int skip = 0; skip < linesToSkip; skip++)
            {
                reader.ReadLine();
            }

            for (int dataLine = 0; dataLine < 5; dataLine++)
            {
                line = reader.ReadLine();

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

    /*
     * === Skill System Techincal Design ===
     * X DatastoreFlowchart needs 20 variables for current stats: 4 stats * 5 characters
     *   X We can conveniently hack this by using instead 5 of the Vector4 datatype. It may even be possible to create our own
     *     5x4 array datatype specifically for storing player stats.
     *     / This new datatype would need to be settable in inspector. Ideally, the inspector should have the player name next to each row
     *       that corresponds to their stats.
     * X DatastoreFlowchart needs an additions 20 variables to store the previous values of all of the stats,
     *   X This way, we can show if a stat increasd or decreased since the last prep phase.
     * 
     * == Code flow ==
     * = Not at runtime =
     * X Add new data properties to DatastoreFlowchart for storing current and previous stat values.
     * X Create a new Stat Manager class for handing stat increases and showing the Stat Changed Popup.
     *   X We will make the Stat Manager and its associated UI a child of the SayDialog. This has the downside of that we can only show 
     *     stat changes while dialogue is running (ie. the SayDialog is visible), but this is ok because we will only be trying to 
     *     change stats through the conversation node.
     * X Create a custom Conversation keyword that will tell the Stat Manager to change a stat. The keyword comes with player name, stat
     *   and the integer change in that stat which can be positive or negative, and can be greater than 1.
     * X Create a Stat Changed Popup prefab UI element to display the change in stats.
     * X Modify ScrimResultsMenu.SetupScrimResults to read from DatastoreFlowchart instead of CSV.
     * - Scrim results CSV should only have notes section now.
     * 
     * = While in dialogue =
     * X Use conversation keyword to change a stat for a player
     *   X The conversation block finds the Stat Manager in the scene and calls a function to change a stat
     * X Stat Manager updates the variable for that player's stat in the scene's DatastoreFlowchart
     *   X Will need to figure out how to change Flowchart variables from code
     * X Stat Manager instantiates a new Stat Changed Popup and tells it whose stat was changed, what stat was changed, and if that
     *   stat went up or down.
     * - Stat Changed Popup animates onto the screen, remains for a set lifetime, animates off the screen, then destroys itself.
     * X Stat Changed Popups will be a child of a vertical scrollbox, which itself is a child of the SayDialog and the parent of 
     *   the Stat Manager script. This will make it easy to scale and organize multiple Stat Changed Popups if multiple stat changes
     *   happen close together.
     * 
     * = Start of prep phase =
     * X ScrimResultsMenu reads from DatastoreFlowchart in SetupScrimResults().
     *   X Set numbers, icons, text, and color of the attribute part of the prep phase menu to reflect the value read.
     *   - Compare to the previous stat values and set an icon showing if the stat went up, down, or (optionally) stayed the same.
     * 
     * = End of prep phase =
     * - Set DatastoreFlowchart previous stat values to the current stat values
     * 
     */
}
