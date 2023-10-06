using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CompositionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Function to call when the mouse starts hovering over this button.
    [SerializeField] private UnityEvent pointerEnterEvent;
    // Function to call when the mouse stops hovering over this button.
    [SerializeField] private UnityEvent pointerExitEvent;

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
}
