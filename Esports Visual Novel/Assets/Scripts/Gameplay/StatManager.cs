using Fungus;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    // The UI element that shows up and displays what stat was changed.
    [SerializeField] private GameObject statChangedPopup;

    private Flowchart datastoreFlowchart;

    // Start is called before the first frame update
    void Start()
    {
        // Bind StatManager.ChangeStat to the changeStatsDelegate of every Conversation node in the scene.
        // Sort of a hack, but it's a way to get the ConversationManager to communicate with StatManager because 
        // ConversationManager doesn't know StatManager exists due to Assembly-CSharp compiling after Fungus library.
        foreach (Conversation conversationNode in FindObjectsOfType<Conversation>())
        {
            conversationNode.changeStatsDelegate = ChangeStat;
        }

        GameObject datastoreGameObject = GameObject.Find("DatastoreFlowchart");
        if (datastoreGameObject != null)
        {
            datastoreFlowchart = datastoreGameObject.GetComponent<Flowchart>();
        }
    }

    /// <summary>
    /// Adds or subtracts from a player's stat spread in the scene's datastore flowchart.
    /// </summary>
    /// <param name="characterName">Choices are Atropos, Boiga, Hajoon, Maeday, and Velocity.</param>
    /// <param name="attribute">The category of stat to change.</param>
    /// <param name="statChange">Any integer change. Will be clamped between stat max and min values.</param>
    public void ChangeStat(string characterName, StatAttribute attribute, int statChange)
    {
        Vector4Variable characterStatsVariable = datastoreFlowchart.GetVariable<Vector4Variable>(characterName + "Stats");
        Vector4 characterStats = characterStatsVariable.Value;

        if (characterStats == null)
        {
            Debug.LogError("Could not find stats variable for " + characterName + ". Check that the character name is correct and that DatastoreFlowchart has a Vector4 stats variable with their name.");
            return;
        }

        // NOTE: Attributes in the Vector4 are listed by how they appear in the game, not in alphabetical order.
        //       Therefore, characterStats.x => Skill
        //                  characterStats.y => Cooperation
        //                  characterStats.z => Communication
        //                  characterStats.w => Attitude
        switch (attribute)
        {
            case StatAttribute.Skill:
                characterStats.x = Mathf.Clamp(characterStats.x + statChange, 1, 5);
                break;
            case StatAttribute.Cooperation:
                characterStats.y = Mathf.Clamp(characterStats.y + statChange, 1, 5);
                break;
            case StatAttribute.Communication:
                characterStats.z = Mathf.Clamp(characterStats.z + statChange, 1, 5);
                break;
            case StatAttribute.Attitude:
                characterStats.w = Mathf.Clamp(characterStats.w + statChange, 1, 5);
                break;
            default:
                Debug.LogError("No functionality for changing attribute " + attribute.ToString() + ". Add functionality to StatManager.ChangeStat if it is missing.");
                break;
        }

        characterStatsVariable.Apply(SetOperator.Assign, characterStats);

        // Make the popup
        GameObject popup = Instantiate(statChangedPopup, transform);
        popup.transform.SetAsFirstSibling();
        popup.GetComponent<StatChangedPopup>().SetText(characterName + "'s " + attribute.ToString() + " " + (statChange > 0 ? "increased" : "decreased"), statChange);
    }
}
