using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class StatManager : MonoBehaviour
{
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
    }

    /// <summary>
    /// Adds or subtracts from a player's stat spread in the scene's datastore flowchart.
    /// </summary>
    /// <param name="characterName">Choices are Atropos, Boiga, Hajoon, Maeday, and Velocity.</param>
    /// <param name="attribute">The category of stat to change.</param>
    /// <param name="statChange">Any integer change. Will be clamped between stat max and min values.</param>
    public void ChangeStat(string characterName, StatAttribute attribute, int statChange)
    {
        print("We have called our delegate");
        //Debug.Log("Changing " + currentCharacter.name + "'s " + item.Stat.ToString() + " attribute by " + item.StatChange.ToString());
    }
}
