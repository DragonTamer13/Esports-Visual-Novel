using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows the Player to select a team composition for the upcoming game. Shows a preview of the players' positions
/// and characters they will be playing.
/// </summary>
public class VODReviewMenuController : MonoBehaviour
{
    // Parent GameObject of this menu's composition choice buttons.
    [SerializeField] private GameObject compositionButtonHolder;
    // The composition preview is shown by enabling different parent objects, each containing a unique arrangement 
    // of the characters for the composition that the preview represents. This GameObject holds the parent objects.
    [SerializeField] private GameObject compositionPreviewHolder;

    // The currently selected composition button.
    private int selection = -1;
    // The preview that is currently being shown. Might not be the same as the current selection.
    private int displayedPreview = -1;
    // Composition button components 
    private List<Button> buttons = new List<Button>();
    // Composition preview objects
    private List<GameObject> previews = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        buttons.AddRange(compositionButtonHolder.transform.GetComponentsInChildren<Button>());
        foreach (Transform t in compositionPreviewHolder.transform) 
        { 
            previews.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Displays a preview associated with a button while hovering over a button, but does not change the selection. 
    /// Call when pointer starts hovering over a composition button.
    /// </summary>
    /// <param name="hoveredButton">The button being hovered over.</param>
    public void PointerEnter(Button hoveredButton)
    {
        if (selection >= 0)
        {
            previews[selection].SetActive(false);
        }
        displayedPreview = buttons.FindIndex(b => b == hoveredButton);
        previews[displayedPreview].SetActive(true);
    }

    /// <summary>
    /// Resets the preview to the current selection, if any.
    /// </summary>
    public void PointerExit()
    {
        previews[displayedPreview].SetActive(false);
        if (selection >= 0)
        {
            previews[selection].SetActive(true);
            displayedPreview = selection;
        }
        else
        {
            displayedPreview = -1;
        }
    }

    /// <summary>
    /// Switch to a new composition preview for a given button. Call when showing a preview while hovering over a button
    /// and for switching to a new preview after pressing a button.
    /// </summary>
    /// <param name="selectedButton">The composition button associated with the preview to show.</param>
    public void SwitchToPreview(Button selectedButton)
    {
        if (selection >= 0)
        {
            previews[selection].SetActive(false);
            buttons[selection].interactable = true;
        }
        selection = buttons.FindIndex(b => b == selectedButton);
        previews[selection].SetActive(true);
        buttons[selection].interactable = false;
        displayedPreview = selection;
    }
}
