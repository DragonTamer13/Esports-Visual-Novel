using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    // All SaveLoadButton components in the menu buttons.
    private List<SaveLoadButton> buttons = new List<SaveLoadButton>();
    private CanvasGroup canvasGroup; // Use a CG to show/hide this menu or else the Buttons' save/load state isn't
                                     // set correctly the first time the menu is opened.

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in transform.Find("SaveLoadButtons").transform)
        {
            buttons.Add(t.GetComponent<SaveLoadButton>());
        }
        canvasGroup = GetComponent<CanvasGroup>();
        Close();
    }

    // Enable and show the menu.
    private void Open()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
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
