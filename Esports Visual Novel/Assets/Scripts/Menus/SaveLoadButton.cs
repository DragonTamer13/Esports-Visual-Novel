using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/// <summary>
/// Controller script for a button in the Save and Load Game menu.
/// Configure to save or load by calling SetIsSaving()
/// </summary>
public class SaveLoadButton : MonoBehaviour
{
    private string saveDataKey = "";
    private MainMenuController mainMenuController;
    private bool isSaving; // True when button saves a game, false when button loads a game.

    // Set the slot this button saves or loads from the index of the save. 
    public void SetSaveDataKey(int saveIndex)
    {
        saveDataKey = "Slot" + saveIndex.ToString();
    }

    public void SetIsSaving(bool value)
    {
        isSaving = value;
    }

    void Start()
    {
        if (GameObject.Find("MainMenu") != null)
        {
            mainMenuController = GameObject.Find("MainMenu").GetComponent<MainMenuController>();
        }
    }

    public void OnClick()
    {
        if (saveDataKey == "")
        {
            Debug.LogError("Attempting to save/load with empty saveDataKey");
            return;
        }

        var saveManager = FungusManager.Instance.SaveManager;

        // Save or load the game when this button is clicked
        if (isSaving)
        {
            if (saveManager.NumSavePoints > 0)
            {
                saveManager.Save(saveDataKey);
            }
            print("Saved the game to " + saveDataKey);
        }
        else
        {
            if (saveManager.SaveDataExists(saveDataKey))
            {
                // Do a fade transition if we're on the main menu. Otherwise, just load the game.
                if (mainMenuController != null)
                {
                    mainMenuController.LoadGame(saveDataKey);
                }
                else
                {
                    saveManager.Load(saveDataKey);
                    transform.parent.parent.gameObject.GetComponent<SaveLoadMenu>().Close();
                }
            }
            else
            {
                Debug.LogError("Attemping to load from invalid save key: " + saveDataKey);
            }
        }
    }
}
