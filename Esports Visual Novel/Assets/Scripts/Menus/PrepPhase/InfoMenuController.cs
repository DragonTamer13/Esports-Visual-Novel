using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Cycles child object visibilty to highlight different sections of the screen.
/// </summary>
public class InfoMenuController : MonoBehaviour
{
    private List<GameObject> covers = new List<GameObject>();
    int currentCover = -1;

    /// <summary>
    /// Highlight the next area of the screen.
    /// </summary>
    public void ShowNextCover()
    {
        // Initialize covers 
        if (covers.Count == 0)
        {
            foreach (Transform t in transform)
            {
                covers.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        if (currentCover >= 0)
        {
            covers[currentCover].SetActive(false);
        }
        currentCover++;
        if (currentCover < covers.Count)
        {
            covers[currentCover].SetActive(true);
        }
        else
        {
            // End cycling if we have shown every cover.
            currentCover = -1;
        }
    }

    /// <summary>
    /// Hide the currently enabled cover.
    /// </summary>
    public void HideCover()
    {
        if (currentCover < 0 || currentCover >= covers.Count)
        {
            return;
        }

        covers[currentCover].SetActive(false);
    }
}
