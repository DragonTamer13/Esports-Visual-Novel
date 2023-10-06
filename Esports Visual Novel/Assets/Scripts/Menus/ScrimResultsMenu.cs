using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrimResultsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject[] panels;
    private int curPanel;

    // Start is called before the first frame update
    void Start()
    {
        curPanel = 0;
        panels[curPanel].SetActive(true);
    }

    public void next()
    {
        if (curPanel + 2 <= panels.Length)
        {
            panels[curPanel].SetActive(false);
            curPanel++;
            panels[curPanel].SetActive(true);
        }
    }
}
