using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatChangedPopup : MonoBehaviour
{
    // How long this popup will show for before going away.
    private const float Lifetime = 3.0f;
    // How long it takes for the popup to fade in or out.
    private const float FadeTime = 0.5f;

    // The popup's text component
    [SerializeField] private TMP_Text text;

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(FadeIn());
    }

    public void SetText(string newText)
    {
        // TODO: Add vars for images. Change signature to tell if the stat went up or down.
        text.text = newText;
    }

    private IEnumerator FadeIn()
    {
        float curFadeTime = 0.0f;

        while (curFadeTime < FadeTime)
        {
            curFadeTime += Time.deltaTime;
            canvasGroup.alpha = curFadeTime / FadeTime;
            yield return null;
        }

        StartCoroutine(WaitUntilFadingOut());
    }

    private IEnumerator WaitUntilFadingOut()
    {
        yield return new WaitForSeconds(Lifetime);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float curFadeTime = 0.0f;

        while (curFadeTime < FadeTime)
        {
            curFadeTime += Time.deltaTime;
            canvasGroup.alpha = 1.0f - curFadeTime / FadeTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
