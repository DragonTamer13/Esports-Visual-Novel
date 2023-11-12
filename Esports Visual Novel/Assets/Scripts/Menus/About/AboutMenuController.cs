using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains main functionality to swap between pages in the About Menu.
/// </summary>
public class AboutMenuController : MonoBehaviour
{
    // Parent object of the menu's page buttons.
    [SerializeField] private Transform pageButtonHolder;

    private List<Button> pageButtons = new List<Button>();
    private Button currentButton = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject g in pageButtonHolder)
        {
            pageButtons.Add(g.GetComponent<Button>());
        }
    }

    /// <summary>
    /// Change the displayed page to match a clicked button.
    /// </summary>
    /// <param name="button">The button that was just clicked.</param>
    public void SwitchToPage(Button button)
    {
        //pageButtons.FindIndex(b => b == button);
        if (currentButton != null && currentButton != button)
        {
            currentButton.interactable = true;
            currentButton = button;
        }
    }
}
