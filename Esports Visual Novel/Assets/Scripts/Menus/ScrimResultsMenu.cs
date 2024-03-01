using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScrimResultsMenu : MonoBehaviour
{
    private readonly string ScrimResultsCSVPath = "/Resources/LivestreamMessages.csv";

    public enum MatchDay
    {
        Day1,
        Day2
    }

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

    /// <summary>
    /// Set up the stats and comments for the players for a desired match day.
    /// </summary>
    /// <param name="matchDay">The day to display scrim results for.</param>
    public void SetupScrimResults(MatchDay matchDay)
    {
        using (StreamReader reader = File.OpenText(Application.dataPath + ScrimResultsCSVPath))
        {
            int deliminator = 0;
            string username = "";
            string message = "";
            string line = reader.ReadLine();
            //line = reader.ReadLine();

            while (line != null)
            {
                // TODO: Copy from LivestreamController.cs for reading and parsing the CSV.
            }
        }
    }

    /**
     * Here's what's gonna happen:
     * - New function here that takes in an enum. Each enum value corresponds to a match day. It uses the enum value to determine
     *   which CSV to read in, then sets the values on the screen based on the CSV information.
     * - A new Fungus command specifically sets up this menu for today. The only exposed variable is the day selector enum. 
     *   When the command is run, call the above function with the value set in inspector.
     */ 
}
