using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class LoadGameButton : MonoBehaviour
{
    // TODO: Make this unique for each save slot button.
    private const string saveDataKey = "Slot0";
    private MainMenuController mainMenuController;

    void Start()
    {
        mainMenuController = GameObject.Find("MainMenu").GetComponent<MainMenuController>();
    }

    public void OnClick()
    {
        var saveManager = FungusManager.Instance.SaveManager;

        if (saveManager.SaveDataExists(saveDataKey))
        {
            mainMenuController.LoadGame(saveDataKey);
        }
    }
}
