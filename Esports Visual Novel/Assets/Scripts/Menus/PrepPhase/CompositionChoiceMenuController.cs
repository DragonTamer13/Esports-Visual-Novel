using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Fungus;

/// <summary>
/// Allows the Player to select a team composition for the upcoming game. Shows a preview of the players' positions
/// and characters they will be playing.
/// </summary>
public class CompositionChoiceMenuController : Menu
{
    // The flowchart for this day.
    [SerializeField] private Flowchart dayFlowchart;
    // Parent GameObject of this menu's composition choice buttons.
    [SerializeField] private GameObject compositionButtonHolder;
    // The composition preview is shown by enabling different parent objects, each containing a unique arrangement 
    // of the characters for the composition that the preview represents. This GameObject holds the parent objects.
    [SerializeField] private GameObject compositionPreviewHolder;
    // Button to leave the menu once a composition is selected.
    [SerializeField] private Button doneButton;
 
    // The currently selected composition button.
    private int selection = -1;
    // The preview that is currently being shown. Might not be the same as the current selection.
    private int displayedPreview = -1;
    // Number of composition options.
    private int activeButtons = 0;
    // Composition selectors
    private List<Toggle> options = new List<Toggle>();
    // Composition preview objects
    private List<GameObject> previews = new List<GameObject>();
    // isWinning[i] == true when button[i] is for a winning composition.
    private List<bool> isWinning = new List<bool>();

    private void Awake()
    {
        if (options.Count <= 0)
        {
            FindCompositionButtons();
        }
        // Attempt to find the day flowchart if it wasn't set in the inspector.
        if (dayFlowchart == null)
        {
            foreach (Flowchart f in GameObject.FindObjectsOfType<Flowchart>())
            {
                if (f.gameObject.name != "DatastoreFlowchart" && f.gameObject.name.Contains("day"))
                {
                    dayFlowchart = f;
                    break;
                }
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        foreach (Transform t in compositionPreviewHolder.transform) 
        { 
            previews.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }

        doneButton.interactable = false;
        Hide();
    }

    /// <summary>
    /// Get all choice buttons that are a child of this object.
    /// </summary>
    private void FindCompositionButtons()
    {
        options.Clear();
        options.AddRange(compositionButtonHolder.transform.GetComponentsInChildren<Toggle>());
        foreach (Toggle t in options)
        {
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
        displayedPreview = options.FindIndex(b => b == hoveredButton);
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
    /// Switch to a new composition preview for a given button. Call when switching to a new preview after pressing a button.
    /// </summary>
    /// <param name="selectedButton">The composition button associated with the preview to show.</param>
    public void SwitchToPreview(Toggle selectedOption)
    {
        if (!selectedOption.isOn)
        {
            return;
        }

        if (selection >= 0)
        {
            previews[selection].SetActive(false);
            options[selection].interactable = true;
        }
        selection = options.FindIndex(b => b == selectedOption);
        previews[selection].SetActive(true);
        options[selection].interactable = false;
        displayedPreview = selection;

        // Update the flowchart variable for the composition wincon.
        if (isWinning[selection])
        {
            dayFlowchart.SendFungusMessage("SetWinningComp");
        }
        else
        {
            dayFlowchart.SendFungusMessage("SetLosingComp");
        }
        doneButton.interactable = true;
    }

    /// <summary>
    /// Makes a button for a composition that the player can choose for this day.
    /// </summary>
    public void CreateCompositionOption(string name, string description, bool winning)
    {
        if (options.Count <= 0)
        {
            FindCompositionButtons();
        }

        options[activeButtons].gameObject.SetActive(true);
        options[activeButtons].GetComponent<CompositionButton>().SetText(name, description);
        isWinning.Add(winning);
        activeButtons++;
    }
}
