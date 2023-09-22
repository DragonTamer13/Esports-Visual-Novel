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
    // The menu controller script associated with this button.
    [SerializeField] private SaveLoadMenu saveLoadMenu;

    private string saveDataKey = "";
    private bool isSaving; // True when button saves a game, false when button loads a game.
    private Text saveNameText;
    private Button button;

    void Start()
    {
        saveNameText = GetComponentInChildren<Text>();
        button = GetComponent<Button>();
    }

    /// <summary>
    /// The name of the save displayed on the button.
    /// </summary>
    public string GetSaveName()
    {
        return saveNameText.text;
    }

    public void OnClick()
    {
        if (saveDataKey == "")
        {
            Debug.LogError("Attempting to save/load with empty saveDataKey");
            return;
        }

        saveLoadMenu.OnButtonClick(saveDataKey, this);
    }

    /// <summary>
    /// Update the save slot index and isSaving value of this button.
    /// Sets button text and interactability to match new values.
    /// </summary>
    public void SetButtonValues(int saveIndex, bool isSaving)
    {
        this.isSaving = isSaving;
        saveDataKey = "Slot" + saveIndex.ToString();

        UpdateButton();
    }

    /// <summary>
    /// Set this button's interactability and text description.
    /// </summary>
    public void UpdateButton()
    {
        var saveManager = FungusManager.Instance.SaveManager;

        // Update button interactability
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

        // NOTE: Not giving much thought to the web saving option.
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
}
