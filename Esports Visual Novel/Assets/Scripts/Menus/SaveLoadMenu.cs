using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadMenu : MonoBehaviour
{
    // Parent object for the save and load UI buttons.
    [SerializeField] private GameObject saveLoadButtonHolder;
    // Parent object for the page UI buttons.
    [SerializeField] private GameObject pageButtonHolder;

    // All SaveLoadButton components in the menu buttons.
    private List<SaveLoadButton> buttons = new List<SaveLoadButton>();
    private List<Button> pageButtons = new List<Button>();
    private CanvasGroup canvasGroup; // Use a CG to show/hide this menu or else the Buttons' save/load state isn't
                                     // set correctly the first time the menu is opened.
    // The current page of the menu screen, zero-indexed.
    private int menuPage = 0;

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

        SetMenuPage(menuPage);
        Close();
    }

    // Enable and show the menu.
    private void Open()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        UpdateSaveSlots();
    }

    // Change which save slots the buttons save/load from based on the current menu page.
    private void UpdateSaveSlots()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetSaveDataKey(menuPage * buttons.Count + i);
        }
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
    /// Open the menu to save game data.
    /// </summary>
    public void OpenSave()
    {
        foreach (SaveLoadButton b in buttons)
        {
            b.SetIsSaving(true);
        }
        Open();
    }

    /// <summary>
    /// Open the menu to load game data.
    /// </summary>
    public void OpenLoad()
    {
        foreach (SaveLoadButton b in buttons)
        {
            b.SetIsSaving(false);
        }
        Open();
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
