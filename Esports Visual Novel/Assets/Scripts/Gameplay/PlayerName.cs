using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the player name for the Character this script is attached to.
/// 
/// NOTE:
/// 1. Have a menu when the player first starts the game using the "New Game" button. Use this to enter their name or something.
/// 2. Save the string using the actual save system within Unity. There will have to be many saves for each of the save slots.
///    A. Maybe after the menu goes away and the game loads for the first time, set some variable in the datastore flowchart. 
///       That way we can sorta hack together a way to save the player name without having to attach "real" saving to the existing
///       save system.
/// 3. Use this script to read the variable from memory/the datastore flowchart and set Character's display name to the stored value.
/// </summary>
public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
