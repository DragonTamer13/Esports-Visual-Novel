using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains main functionality to swap between pages in the About Menu.
/// 
/// Can be generalized to any menu, so long as each navigation button has events that call SwitchToPage and 
/// the order of the page buttons in the hierarchy is the same as the order of the pages. ie. the ith child
/// of buttonHolder corresponds to the ith child of pageHolder.
/// </summary>
public class AboutMenuController : MonoBehaviour
{
    // Parent object of the menu's page buttons.
    [SerializeField] private Transform buttonHolder;
    // Parent object of the menu's different pages.
    [SerializeField] private Transform pageHolder;

    private List<Button> pageButtons = new List<Button>();
    private List<GameObject> pages = new List<GameObject>();
    private Button currentButton = null;
    private GameObject currentPage = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in buttonHolder)
        {
            pageButtons.Add(t.GetComponent<Button>());
        }
        foreach (Transform t in pageHolder)
        {
            pages.Add(t.gameObject);
        }

        SwitchToPage(pageButtons[0]);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Change the displayed page to match a clicked button.
    /// </summary>
    /// <param name="button">The button that was just clicked.</param>
    public void SwitchToPage(Button button)
    {
        if (currentButton != button)
        {
            if (currentButton != null)
            {
                currentButton.interactable = true;
                currentPage.SetActive(false);
            }
            button.interactable = false;
            currentButton = button;
            currentPage = pages[pageButtons.FindIndex(b => b == button)];
            currentPage.SetActive(true);
        }
    }
}
