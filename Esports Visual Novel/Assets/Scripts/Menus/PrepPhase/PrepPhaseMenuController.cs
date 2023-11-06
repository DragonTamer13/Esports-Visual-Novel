using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepPhaseMenuController : MonoBehaviour
{
    // The Save Menu UI in this scene.
    [SerializeField] private GameObject saveMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (saveMenu == null)
        {
            saveMenu = GameObject.Find("SaveMenu");
        }
    }

    public void ShowSaveMenu()
    {
        saveMenu.SetActive(true);
    }

    public void HideSaveMenu()
    {
        saveMenu.SetActive(false);
    }
}
