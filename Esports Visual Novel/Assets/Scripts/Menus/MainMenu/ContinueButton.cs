using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

/// <summary>
/// Functionality specifically for the Continue Game button on the main menu.
/// Loads the most recent save when clicked.
/// </summary>
public class ContinueButton : MonoBehaviour
{
    private MainMenuController mainMenuController;
    private string saveDataKey = "";

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("MainMenu") != null)
        {
            mainMenuController = GameObject.Find("MainMenu").GetComponent<MainMenuController>();
        }

        DateTime latestWriteTime = DateTime.MinValue;
        DateTime writeTime;
        int slots = GameObject.Find("SaveLoadMenu").GetComponent<SaveLoadMenu>().GetNumSaveSlots();

        for (int i = 0; i < slots; i++)
        {
            string keyToCheck = "Slot" + i;
            var fullFilePath = SaveManager.GetFullFilePath(keyToCheck);
            if (System.IO.File.Exists(fullFilePath))
            {
                writeTime = System.IO.File.GetLastWriteTime(fullFilePath);
                if (writeTime > latestWriteTime)
                {
                    latestWriteTime = writeTime;
                    saveDataKey = keyToCheck;
                }
            }
        }

        if (saveDataKey == "")
        {
            GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Load the most recent save.
    /// </summary>
    public void OnClick()
    {
        var saveManager = FungusManager.Instance.SaveManager;

        if (saveManager.SaveDataExists(saveDataKey))
        {
            mainMenuController.LoadGame(saveDataKey);
        }
        else
        {
            Debug.LogError("Attemping to load from invalid save key: " + saveDataKey);
        }
    }
}
