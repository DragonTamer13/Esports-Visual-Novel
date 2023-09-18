using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float currentFadeTime = 0.0f;

        fadeCover.gameObject.SetActive(true);
        while (currentFadeTime < FadeTime)
        {
            currentFadeTime += Time.deltaTime;
            fadeCover.alpha = currentFadeTime / FadeTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(sceneName);
    }
}
