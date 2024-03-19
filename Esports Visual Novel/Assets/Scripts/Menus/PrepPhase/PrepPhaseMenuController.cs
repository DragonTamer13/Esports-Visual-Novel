using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepPhaseMenuController : MonoBehaviour
{
    public enum MatchDay
    {
        Tuesday_1,
        Wednesday_2,
        Thursday_3,
        Friday_4_vs_3
    }

    // The Save Menu UI in this scene.
    [SerializeField] private GameObject saveMenu;
    [SerializeField] private ScrimResultsMenu scrimResultsMenu;
    [SerializeField] private SocialMediaController socialMediaController;

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        if (saveMenu == null)
        {
            saveMenu = GameObject.Find("SaveMenu");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    public void ShowSaveMenu()
    {
        saveMenu.SetActive(true);
    }

    public void HideSaveMenu()
    {
        saveMenu.SetActive(false);
    }

    /// <summary>
    /// Show the prep phase menu.
    /// </summary>
    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide the prep phase menu.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Sets up all the data that is displayed for today on the prep phase sub-menus: the bracket, Deets, and scrim results menus.
    /// </summary>
    /// <param name="matchDay">The day to set up for.</param>
    public void SetUpPrepPhase(MatchDay matchDay)
    {
        scrimResultsMenu.SetupScrimResults(matchDay);
        socialMediaController.SetSocialMediaPosts(matchDay);
    }
}
