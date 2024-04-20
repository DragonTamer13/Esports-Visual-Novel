using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

/// <summary>
/// Controller script for a button in the Save and Load Game menu.
/// Configure to save or load by calling SetIsSaving()
/// </summary>
public class SaveLoadButton : MonoBehaviour
{
    // The menu controller script associated with this button.
    [SerializeField] private SaveLoadMenu saveLoadMenu;
    // The image component associated with this save file.
    [SerializeField] private Image saveGameImage;
    // This button's smaller delete button.
    [SerializeField] private Button deleteButton;
    [SerializeField] private TMP_Text saveNameText;
    [SerializeField] private TMP_Text saveDateText;

    private string saveDataKey = "";
    private bool isSaving; // True when button saves a game, false when button loads a game.
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// The name of the save displayed on the button.
    /// </summary>
    public string GetSaveName()
    {
        // TODO: Fix this. Parse date into two strings.
        return saveNameText.text + " " +saveDateText.text;
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

    public void OnDeleteClick()
    {
        if (saveDataKey == "")
        {
            Debug.LogError("Attempting to delete an empty saveDataKey");
            return;
        }

        saveLoadMenu.OnDeleteClick(saveDataKey, this);
    }

    /// <summary>
    /// Update the save slot index and isSaving value of this button.
    /// Sets button text and interactability to match new values.
    /// </summary>
    public void SetButtonValues(int saveIndex, bool isSaving)
    {
        this.isSaving = isSaving;

        if (saveIndex < Autosave.MaxAutosaves)
        {
            saveDataKey = Autosave.AutosavePrefix + saveIndex.ToString();
        }
        else
        {
            // The first save menu pages are reserved for autosaves, so change the indexing for later save slots.
            saveDataKey = "Slot" + (saveIndex - Autosave.MaxAutosaves).ToString();
        }

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
            button.interactable = isSaving;
            deleteButton.interactable = false;
            saveNameText.text = "No save";
            saveGameImage.sprite = null;
            return;
        }
        else
        {
            button.interactable = true;
            deleteButton.interactable = true;
        }

        // Update the text on the button to show a description of the save.
        // From SaveManager.ReadSaveHistory()
        var historyData = string.Empty;

        // NOTE: Not giving much thought to the web saving option.
#if UNITY_WEBPLAYER || UNITY_WEBGL
            historyData = PlayerPrefs.GetString(saveDataKey);
#else
        var fullFilePath = SaveManager.GetFullFilePath(saveDataKey);
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

        // Load the image for this save slot if it exists.
        string imagePath = Application.persistentDataPath + "/" + saveDataKey + ".png";
        if (System.IO.File.Exists(imagePath))
        {
            Texture2D texture = new Texture2D(2, 2);
            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
            texture.LoadImage(imageData);
            Sprite image = Sprite.Create(texture,
                                         new Rect(0f, 0f, texture.width, texture.height),
                                         new Vector2(0.5f, 0.5f));
            saveGameImage.sprite = image;
        }
        else
        {
            saveGameImage.sprite = null;
        }
    }
}
