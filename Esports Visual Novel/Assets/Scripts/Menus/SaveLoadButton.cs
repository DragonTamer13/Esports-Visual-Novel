using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private Text saveNameText;
    private Button button;

    // Set the slot this button saves or loads from the index of the save. 
    public void SetSaveDataKey(int saveIndex)
    {
        saveDataKey = "Slot" + saveIndex.ToString();

        UpdateButton();
    }

    public void SetIsSaving(bool value)
    {
        isSaving = value;

        // Set if the button is interactable based on isSaving and if save data exists for this slot already.
        var saveManager = FungusManager.Instance.SaveManager;
        if (!saveManager.SaveDataExists(saveDataKey) && !isSaving)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    void Start()
    {
        saveNameText = GetComponentInChildren<Text>();
        button = GetComponent<Button>();

        if (GameObject.Find("MainMenu") != null)
        {
            mainMenuController = GameObject.Find("MainMenu").GetComponent<MainMenuController>();
        }
    }

    /// <summary>
    /// Set this button's interactability and text description.
    /// </summary>
    private void UpdateButton()
    {
        var saveManager = FungusManager.Instance.SaveManager;
        if (!saveManager.SaveDataExists(saveDataKey))
        {
            if (!isSaving)
            {
                button.interactable = false;
            }
            saveNameText.text = "No save";
            return;
        }
        else
        {
            button.interactable = true;
        }

        // Update the text on the button to show a description of the save.
        // From SaveManager.ReadSaveHistory()
        var historyData = string.Empty;

#if UNITY_WEBPLAYER || UNITY_WEBGL
            historyData = PlayerPrefs.GetString(saveDataKey);
#else
        var fullFilePath = SaveManager.STORAGE_DIRECTORY + saveDataKey + ".json";
        if (System.IO.File.Exists(fullFilePath))
        {
            historyData = System.IO.File.ReadAllText(fullFilePath);
        }
#endif//UNITY_WEBPLAYER
        if (!string.IsNullOrEmpty(historyData))
        {
            var tempSaveHistory = JsonUtility.FromJson<SaveHistory>(historyData);
            if (tempSaveHistory != null)
            {
                // JSON of save data
                var savePointData = JsonUtility.FromJson<SavePointData>(tempSaveHistory.GetLastSavePoint());
                saveNameText.text = savePointData.SavePointKey + " (" + savePointData.SavePointDescription + ")";
            }
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
            // Show new save name on button.
            UpdateButton();
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
