using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    // The parent menu object.
    [SerializeField] private GameObject saveMenu;

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void OnClick()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(saveMenu);
    }
}
