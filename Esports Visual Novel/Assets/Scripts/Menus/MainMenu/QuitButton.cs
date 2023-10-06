using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    // The parent menu object.
    [SerializeField] private GameObject saveMenu;

    // Menu that pops up when the Quit button is pressed.
    [SerializeField] private GameObject quitConfirmationMenu;

    void Start()
    {
        quitConfirmationMenu.SetActive(false);
    }

    /// <summary>
    /// Shows the Quit confirmation menu before actually quitting the game.
    /// </summary>
    public void OnClick()
    {
        quitConfirmationMenu.SetActive(true);
    }

    /// <summary>
    /// Return to the main menu. Should only be called by the Quit confirmation menu button.
    /// </summary>
    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(saveMenu);
    }
}
