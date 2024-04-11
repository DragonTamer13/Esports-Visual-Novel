using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A UI windows that can be shown or hidden using a CanvasGroup component.
/// </summary>
public class Menu : MonoBehaviour
{
    // Will this menu be disabled when the game starts?
    [SerializeField] protected bool startHidden = true;

    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (startHidden)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    /// <summary>
    /// Make this menu visible and intertactable.
    /// </summary>
    public virtual void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Make this menu invisible and unintertactable.
    /// </summary>
    public virtual void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
