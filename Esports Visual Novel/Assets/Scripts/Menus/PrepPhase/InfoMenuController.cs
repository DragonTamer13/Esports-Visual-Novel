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

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in transform)
        {
            covers.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Highlight the next area of the screen.
    /// </summary>
    public void ShowNextCover()
    {
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
}
