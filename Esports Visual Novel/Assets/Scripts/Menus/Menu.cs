using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A UI windows that can be shown or hidden using a CanvasGroup component.
/// </summary>
public class Menu : MonoBehaviour
{
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        Hide();
    }

    /// <summary>
    /// Make this menu visible and intertactable.
    /// </summary>
    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Make this menu invisible and unintertactable.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
