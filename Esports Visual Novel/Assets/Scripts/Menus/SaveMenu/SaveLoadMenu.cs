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
    // Parent object for the save and load UI buttons.
    [SerializeField] private GameObject saveLoadButtonHolder;
    // Parent object for the page UI buttons.
    [SerializeField] private GameObject pageButtonHolder;
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

        // Show new save name on button.
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
        /// <summary>
        /// TODO: More saving bullshit.
        /// Premise: We will split the game into multiple scenes (one for each day), each with its own flowchart. This is so that 
        /// multiple people can work on different days without issue. Also, it prevents having multiple flowcharts or one big 
        /// flowchart in memory while playing the game.
        /// 
        /// Problem: Need a way for a flowchart to access the variables set in another flowchart in another scene.
        /// 1. Can't put all the flowcharts in one scene because it conflicts with the premise and you can't access the variables of
        ///    an inactive flowchart.
        /// 2. Make a "datastore flowchart": Variables needed between flowcharts will all be stored in a flowchart with no nodes.
        ///    We will make a bunch of public variables in this flowchart and make it a prefab so these changes apply across all scenes.
        ///    a. Will need to change saving and loading so that we also save the information in the datastore each time we save/load.
        ///       When a button is clicked to save, use Unity's native saving interface to save the datastore's state somehow. When
        ///       loading, as soon as we get into the new scene, somehow tell the game which save slot was loaded and set the
        ///       datastore's variables to the values saved in memory. Can be done easier if we can somehow call the "Save Point"
        ///       and "OnSavepointLoaded" flowchart commands from another script.
        /// 3. Use a Lua Store to save data
        ///    a. Essentially the same as the datastore flowchart, except we then need an extra step to access data. Variables in the
        ///       individual day flowcharts will be public. They will be set on load by copying from the datastore, which in turn has
        ///       variables copied from the data stored on disk. More complicated than the datastore flowchart.
        ///       
        /// From this, despite its hackish appearance, using an empty flowchart prefab just to store public variables between scenes
        /// is the best way to go. We now have these things to figure out:
        /// 1. Find a way to save a second flowchart whenever we save the game.
        /// 2. Tell a newly-loaded scene which save slot its associated with and which datastore to load.
        /// 3. Set the variables of a flowchart from a script.
        /// </summary>
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
    /// Saves the game to a slot that already has data. Should only be called by the OverwriteConfirmMenu.
    /// </summary>
    public void ConfirmOverwrite()
    {
        // NOTE: This should probably be done using delegates or UnityEvents or callbacks, something like that. Maybe we can change it to work like that later.
        Save(storedKey, storedSaveLoadButton);
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
