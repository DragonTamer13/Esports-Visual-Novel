using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSwap : MonoBehaviour
{
    //Current Button Sprite
    public GameObject button;

    //Sprite to be used when the button is selected/pressed
    public Sprite selectedSprite;

    //Sprite to be used when the button is not selected/pressed
    public Sprite deselectedSprite;

    //Function to be called On Click() 
    public virtual void ToggleButton()
    {
        if (button.GetComponent<Image>().sprite == selectedSprite) 
        { 
            button.GetComponent<Image>().sprite = deselectedSprite; 
        }

        else if (button.GetComponent<Image>().sprite == deselectedSprite) 
        { 
            button.GetComponent<Image>().sprite = selectedSprite; 
        }
    }
}
