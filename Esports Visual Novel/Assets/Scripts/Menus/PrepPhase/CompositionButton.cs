using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class CompositionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Function to call when the mouse starts hovering over this button.
    [SerializeField] private UnityEvent pointerEnterEvent;
    // Function to call when the mouse stops hovering over this button.
    [SerializeField] private UnityEvent pointerExitEvent;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

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

    public void SetText(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }
}
