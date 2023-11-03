using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Special class for a UnityEvent that has one Button component argument.
/// </summary>
public class ButtonEvent : UnityEvent<Button>
{
}

public class CompositionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Function to call when the mouse starts hovering over this button.
    [SerializeField] private UnityEvent pointerEnterEvent;
    // Function to call when the mouse stops hovering over this button.
    [SerializeField] private UnityEvent pointerExitEvent;

    private ButtonEvent onClickEvent;

    public void SetOnClickEvent(ButtonEvent newEvent)
    {
        onClickEvent = newEvent;
    }

    // When highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterEvent.Invoke();
    }

    // Stops being highlighted by the mouse.
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExitEvent.Invoke();
    }

    public void OnClick()
    {
        onClickEvent.Invoke(GetComponent<Button>());
    }
}
