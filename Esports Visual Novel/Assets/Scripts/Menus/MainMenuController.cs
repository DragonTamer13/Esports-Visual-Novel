using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    /// <summary>
    /// Closes the game. Also stops playing in editor if not running the built game.
    /// </summary>
    public void OnQuitGamePressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
