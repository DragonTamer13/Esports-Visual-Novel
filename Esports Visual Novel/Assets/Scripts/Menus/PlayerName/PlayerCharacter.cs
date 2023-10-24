using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/// <summary>
/// Same as the default Character class, except with the ability to set Name Text. Breaks some Character functionality
/// involving finding sprites, so don't have sprites associated with this Character.
/// </summary>
public class PlayerCharacter : Character
{
    public void SetNameText(string newName)
    {
        nameText = newName;
    }
}
