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
}
