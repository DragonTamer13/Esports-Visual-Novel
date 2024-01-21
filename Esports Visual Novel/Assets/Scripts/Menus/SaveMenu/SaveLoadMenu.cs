using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

/// <summary>
/// The main controller for saving and loading the game.
/// </summary>
public class SaveLoadMenu : MonoBehaviour
{
    // Constants
    private const int MaxAutosaves = 4;
    private readonly string AutosavePrefix = "auto";


    // Parent object for the save and load UI buttons.
    [SerializeField] private GameObject saveLoadButtonHolder;
    // Parent object for the page UI buttons.
    [SerializeField] private GameObject pageButtonHolder;
    // Menu for confirming if the player wants to delete a save.
    [SerializeField] private GameObject deleteConfirmMenu;
    // Text object displaying the save to be deleted.
    [SerializeField] private Text deleteConfirmMenuText;
    // Menu for confirming if the player wants to overwrite an existing save.
    [SerializeField] private GameObject overwriteConfirmMenu;
    // Text object on the overwrite menu displaying the name of the save to be overwritten.
    [SerializeField] private Text overwriteConfirmMenuText;

    // All SaveLoadButton components in the menu buttons.
    private List<SaveLoadButton> buttons = new List<SaveLoadButton>();
    private List<Button> pageButtons = new List<Button>();
    private CanvasGroup canvasGroup; // Use a CanvasGroup to show/hide this menu or else the Buttons' save/load state isn't
                                     // set correctly the first time the menu is opened.
    // The current page of the menu screen, zero-indexed.
    private int menuPage = 0;
    private bool isSaving;
    private string storedKey = ""; // Store saveDataKey, to be used when confirming overwriting a save.
    private SaveLoadButton storedSaveLoadButton;

    // Zero-indexed page of the menu.
    public void SetMenuPage(int value)
    {
        if (value < 0 || value >= pageButtons.Count)
        {
            Debug.LogError("Page " + value + " out of range.");
            return;
        }

        // Enable/disable page buttons based on which page the player switched to.
        pageButtons[menuPage].interactable = true;
        pageButtons[value].interactable = false;

        menuPage = value;
        UpdateSaveSlots();
    }

    private static string GetSaveImageName(string saveDataKey)
    {
        return Application.persistentDataPath + "/" + saveDataKey + ".png";
    }

    private void Awake()
    {
        // Counting children here to prevent race condition when calling GetNumSaveSlots().
        // Get all SaveLoadButtons
        foreach (Transform t in saveLoadButtonHolder.transform)
        {
            buttons.Add(t.GetComponent<SaveLoadButton>());
        }

        // Get all page buttons
        foreach (Transform t in pageButtonHolder.transform)
        {
            pageButtons.Add(t.GetComponent<Button>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        overwriteConfirmMenu.SetActive(false);

        SetMenuPage(menuPage);
        Close();
    }

    // Change which save slots the buttons save/load from based on the current menu page.
    private void UpdateSaveSlots()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetButtonValues(menuPage * buttons.Count + i, isSaving);
        }
    }

    private void Save(string saveDataKey, SaveLoadButton saveLoadButton)
    {
        var saveManager = FungusManager.Instance.SaveManager;
        saveManager.Save(saveDataKey);

        StartCoroutine(TakeSaveScreenshot(saveDataKey, saveLoadButton));

        // Show new save name on button.
        saveLoadButton.UpdateButton();
    }

    /// <summary>
    /// Captures a screenshot of the game to display to the player for a save slot.
    /// 
    /// TODO: To fix the flicker, possibly save a temp image when the "Save Game" button is clicked, then create copies of that
    /// image when saving to a save slot and rename the file. Delete the temp image when the menu is closed.
    /// </summary>
    private IEnumerator TakeSaveScreenshot(string saveDataKey, SaveLoadButton saveLoadButton)
    {
        float canvasAlpha = canvasGroup.alpha;

        canvasGroup.alpha = 0;
        yield return new WaitForEndOfFrame();

        ScreenCapture.CaptureScreenshot(GetSaveImageName(saveDataKey));
        canvasGroup.alpha = canvasAlpha;
        yield return new WaitForEndOfFrame();

        saveLoadButton.UpdateButton();
    }

    /// <summary>
    /// Returns the total number of save slots, which is equal to the number of SaveLoadButtons times the number of pages.
    /// </summary>
    /// <returns></returns>
    public int GetNumSaveSlots()
    {
        return buttons.Count * pageButtons.Count;
    }

    /// <summary>
    /// Enable and show the menu.
    /// </summary>
    /// <param name="isSaving">True if opening the save menu, false if opening the load menu.</param>
    public void Open(bool isSaving)
    {
        this.isSaving = isSaving;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        UpdateSaveSlots();
    }

    /// <summary>
    /// Saves or loads game data. Call when a SaveLoadButton is clicked.
    /// </summary>
    /// <param name="saveDataKey">The clicked save slot key.</param>
    /// <param name="saveLoadButton">The button that was clicked.</param>
    public void OnButtonClick(string saveDataKey, SaveLoadButton saveLoadButton)
    {
        var saveManager = FungusManager.Instance.SaveManager;

        // Save or load the game when this button is clicked
        if (isSaving)
        {
            if (saveManager.NumSavePoints > 0)
            {
                // Confirm if player wants to save if data already exists for this save slot.
                if (saveManager.SaveDataExists(saveDataKey))
                {
                    storedKey = saveDataKey;
                    storedSaveLoadButton = saveLoadButton;
                    overwriteConfirmMenuText.text = "Are you sure you want to overwrite \"" + saveLoadButton.GetSaveName() + "\"?";
                    overwriteConfirmMenu.SetActive(true);
                }
                else
                {
                    Save(saveDataKey, saveLoadButton);
                }
            }
        }
        else
        {
            if (saveManager.SaveDataExists(saveDataKey))
            {
                // Do a fade transition if we're on the main menu. Otherwise, just load the game.
                GameObject mainMenu = GameObject.Find("MainMenu");
                if (mainMenu != null)
                {
                    mainMenu.GetComponent<MainMenuController>().LoadGame(saveDataKey);
                }
                else
                {
                    saveManager.Load(saveDataKey);
                    Close();
                }
            }
            else
            {
                Debug.LogError("Attemping to load from invalid save key: " + saveDataKey);
            }
        }
    }

    /// <summary>
    /// Attempt to delete a save file.
    /// </summary>
    /// <param name="saveDataKey">The selected save slot key.</param>
    /// <param name="saveLoadButton">The button that was clicked.</param>
    public void OnDeleteClick(string saveDataKey, SaveLoadButton saveLoadButton)
    {
        var saveManager = FungusManager.Instance.SaveManager;

        if (saveManager.SaveDataExists(saveDataKey))
        {
            storedKey = saveDataKey;
            storedSaveLoadButton = saveLoadButton;
            deleteConfirmMenuText.text = "Are you sure you want to permanently delete \"" + saveLoadButton.GetSaveName() + "\"?";
            deleteConfirmMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Saves the game to a slot that already has data. Should only be called by the OverwriteConfirmMenu.
    /// </summary>
    public void ConfirmOverwrite()
    {
        // NOTE: This should probably be done using delegates or UnityEvents or callbacks, something like that. Maybe we can change it to work like that later.
        Save(storedKey, storedSaveLoadButton);
    }

    /// <summary>
    /// Delete a save file.
    /// </summary>
    public void ConfirmDelete()
    {
        DeleteSave(storedKey);
        storedSaveLoadButton.UpdateButton();
    }

    /// <summary>
    /// Delete a save file and its associated image.
    /// </summary>
    private void DeleteSave(string saveDataKey)
    {
        SaveManager.Delete(saveDataKey);
        if (System.IO.File.Exists(GetSaveImageName(saveDataKey)))
        {
            System.IO.File.Delete(GetSaveImageName(saveDataKey));
        }
    }

    /*
    * How to autosave:
    * 1. Shift existing autosave names down one by changing their names. Delete the last one if it exists already.
    * 2. Also shift preview screenshot names and delete the last one if it exists.
    * 3. Create a save and screenshot with the autosave name.
    * 
    * 1. Shifting names can be done anywhere, as any script can access SaveManager.Instance and change the file names.
    * 2. Same goes for shifting the names.
    * 3. Creating a screenshot is done in SaveLoadMenu, which disables the UI.
    * Therefore, the problem is taking a screenshot, specifically disabling the menu UI.
    * 
    * New Idea: Sometimes the autosave should be done after the screen fades in. The SavePoint command comes before the Fade command.
    * Therefore, there should be a separate command for autosaving. It should be a function call to SaveLoadMenu. Would solve a lot of problems,
    * but forces you to make a new command after each node that has a save slot. Could maybe make a custom command to autosave later on.
    */
    /// <summary>
    /// Create a new autosave file. The oldest autosave is deleted if there are too many saves.
    /// Call this from a flowchart.
    /// </summary>
    public void Autosave()
    {
        var saveManager = FungusManager.Instance.SaveManager;
        string lastAutosaveName = AutosavePrefix + (MaxAutosaves - 1).ToString();

        if (saveManager.SaveDataExists(lastAutosaveName))
        {
            DeleteSave(lastAutosaveName);
        }

        // Shift the saves down
        string toName = "";
        string fromName = lastAutosaveName;
        for (int i = MaxAutosaves - 2; i > -1; i--)
        {
            toName = fromName;
            fromName = AutosavePrefix + i.ToString();
            if (saveManager.SaveDataExists(fromName))
            {
                System.IO.File.Move(SaveManager.GetFullFilePath(fromName), SaveManager.GetFullFilePath(toName));
                System.IO.File.Move(GetSaveImageName(fromName), GetSaveImageName(toName));
            }
        }

        // Create the new autosave
        //Save(AutosavePrefix + "0", null);
    }

    /// <summary>
    /// Disable and hide the menu.
    /// </summary>
    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
