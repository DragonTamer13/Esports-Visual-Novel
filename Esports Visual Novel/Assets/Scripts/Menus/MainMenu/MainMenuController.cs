using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class MainMenuController : MonoBehaviour
{
    // How long it takes for the screen to fade out when switching scenes.
    private const float FadeTime = 1.0f;

    // Object that is used to fade out the screen.
    [SerializeField] private CanvasGroup fadeCover;

    void Start()
    {
        fadeCover.alpha = 0.0f;
        fadeCover.gameObject.SetActive(false);
    }

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

    // Transition to a new scene by fading out the screen.
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName, false));
    }

    // Fade out the screen and load a saved game.
    public void LoadGame(string saveDataKey)
    {
        StartCoroutine(FadeOut(saveDataKey, true));
    }

    /// <summary>
    /// Fade the screen, then either start a new game or load a saved game.
    /// </summary>
    /// <param name="sceneOrSaveName">The scene or name of the save game to load. Save validity should be checked before calling this funciton.</param>
    /// <param name="isLoadingGame">True when loading a save, false when opening a scene.</param>
    /// <returns></returns>
    private IEnumerator FadeOut(string sceneOrSaveName, bool isLoadingGame)
    {
        float currentFadeTime = 0.0f;

        fadeCover.gameObject.SetActive(true);
        while (currentFadeTime < FadeTime)
        {
            currentFadeTime += Time.deltaTime;
            fadeCover.alpha = currentFadeTime / FadeTime;
            yield return new WaitForEndOfFrame();
        }

        if (isLoadingGame)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Load(sceneOrSaveName);
        }
        else
        {
            SceneManager.LoadScene(sceneOrSaveName);
        }
    }
}
