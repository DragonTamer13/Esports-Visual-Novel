using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TestSave : MonoBehaviour
{
    private const string saveDataKey = "Slot0";

    /// <summary>
    /// Handler function called when the Save button is pressed.
    /// </summary>
    public virtual void Save()
    {
        var saveManager = FungusManager.Instance.SaveManager;

        if (saveManager.NumSavePoints > 0)
        {
            saveManager.Save(saveDataKey);
        }
    }
}
