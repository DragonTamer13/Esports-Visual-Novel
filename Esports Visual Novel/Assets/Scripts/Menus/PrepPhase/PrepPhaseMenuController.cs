using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class PrepPhaseMenuController : MonoBehaviour
{
    public enum MatchDay
    {
        Tuesday_1,
        Wednesday_2,
        Thursday_3,
        Friday_4_vs_3,
        Saturday_5_vs_7,
        Sunday_6,
        WeekBreak_7,
        Thursday_8_vs_4,
        Credits
    }

    // The Save Menu UI in this scene.
    [SerializeField] private GameObject saveMenu;
    [SerializeField] private ScrimResultsMenu scrimResultsMenu;
    [SerializeField] private SocialMediaController socialMediaController;

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        if (saveMenu == null)
        {
            saveMenu = GameObject.Find("SaveMenu");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    /// <summary>
    /// Returns true if all field of A are greater than or equal to their corresponding fields in B.
    /// </summary>
    /// <returns></returns>
    private static bool CompareStats(Vector4 A, Vector4 B)
    {
        return A.w >= B.w 
            && A.x >= B.x 
            && A.y >= B.y 
            && A.z >= B.z;
    }

    /// <summary>
    /// Returns the number of fields of A that are greater than or equal to their corresponding fields in B.
    /// </summary>
    private static int CountStats(Vector4 A, Vector4 B)
    {
        int result = 0;

        if (A.w >= B.w)
            result++;
        if (A.x >= B.x)
            result++;
        if (A.y >= B.y)
            result++;
        if (A.z >= B.z)
            result++;

        return result;
    }

    public void ShowSaveMenu()
    {
        if (saveMenu == null)
        {
            return;
        }

        saveMenu.SetActive(true);
    }

    public void HideSaveMenu()
    {
        if (saveMenu == null)
        {
            return;
        }

        saveMenu.SetActive(false);
    }

    /// <summary>
    /// Show the prep phase menu.
    /// </summary>
    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide the prep phase menu.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Sets up all the data that is displayed for today on the prep phase sub-menus: the bracket, Deets, and scrim results menus.
    /// </summary>
    /// <param name="matchDay">The day to set up for.</param>
    /// <param name="teamName">The player team's name.</param>
    public void SetUpPrepPhase(MatchDay matchDay, string teamName)
    {
        scrimResultsMenu.SetupScrimResults(matchDay);
        socialMediaController.SetSocialMediaPosts(matchDay, teamName);
    }

    /// <summary>
    /// Returns true if all players' stats are greater than or equal to a threshold. DatastoreFlowchart is used to get the players' current
    /// stats, while DayFlowchart is used to get the threshold values, formatted [CharacterName]Target; ie. "AtroposTarget".
    /// </summary>
    public bool CheckAllStatsAboveThreshold(Flowchart datastoreFlowchart, Flowchart dayFlowchart)
    {
        Vector4 AtroposCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("AtroposStats").Value;
        Vector4 BoigaCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("BoigaStats").Value;
        Vector4 HajoonCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("HajoonStats").Value;
        Vector4 MaedayCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("MaedayStats").Value;
        Vector4 VelocityCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("VelocityStats").Value;

        Vector4 AtroposTarget = dayFlowchart.GetVariable<Vector4Variable>("AtroposTarget").Value;
        Vector4 BoigaTarget = dayFlowchart.GetVariable<Vector4Variable>("BoigaTarget").Value;
        Vector4 HajoonTarget = dayFlowchart.GetVariable<Vector4Variable>("HajoonTarget").Value;
        Vector4 MaedayTarget = dayFlowchart.GetVariable<Vector4Variable>("MaedayTarget").Value;
        Vector4 VelocityTarget = dayFlowchart.GetVariable<Vector4Variable>("VelocityTarget").Value;

        return CompareStats(AtroposCurrent, AtroposTarget)
            && CompareStats(BoigaCurrent, BoigaTarget)
            && CompareStats(HajoonCurrent, HajoonTarget)
            && CompareStats(MaedayCurrent, MaedayTarget)
            && CompareStats(VelocityCurrent, VelocityTarget);
    }

    /// <summary>
    /// Returns the number of stat attributes that are greater than or equal to a threshold. DatastoreFlowchart is used to get the players' 
    /// current stats, while DayFlowchart is used to get the threshold values, formatted [CharacterName]Target; ie. "AtroposTarget".
    /// </summary>
    public int CountStatsAboveThreshold(Flowchart datastoreFlowchart, Flowchart dayFlowchart)
    {
        Vector4 AtroposCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("AtroposStats").Value;
        Vector4 BoigaCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("BoigaStats").Value;
        Vector4 HajoonCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("HajoonStats").Value;
        Vector4 MaedayCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("MaedayStats").Value;
        Vector4 VelocityCurrent = datastoreFlowchart.GetVariable<Vector4Variable>("VelocityStats").Value;

        Vector4 AtroposTarget = dayFlowchart.GetVariable<Vector4Variable>("AtroposTarget").Value;
        Vector4 BoigaTarget = dayFlowchart.GetVariable<Vector4Variable>("BoigaTarget").Value;
        Vector4 HajoonTarget = dayFlowchart.GetVariable<Vector4Variable>("HajoonTarget").Value;
        Vector4 MaedayTarget = dayFlowchart.GetVariable<Vector4Variable>("MaedayTarget").Value;
        Vector4 VelocityTarget = dayFlowchart.GetVariable<Vector4Variable>("VelocityTarget").Value;

        return CountStats(AtroposCurrent, AtroposTarget)
            + CountStats(BoigaCurrent, BoigaTarget)
            + CountStats(HajoonCurrent, HajoonTarget)
            + CountStats(MaedayCurrent, MaedayTarget)
            + CountStats(VelocityCurrent, VelocityTarget);
    }

    /// <summary>
    /// Sets some of the values of a stat Vector4 instead of all of them. 
    /// </summary>
    public Vector4 SetPartialStats(Vector4 stats, int newSkill, int newCooperation, int newCommunication, int newAttitude)
    {
        if (newSkill > 0)
        {
            stats.w = newSkill;
        }
        if (newCooperation > 0)
        {
            stats.x = newCooperation;
        }
        if (newCommunication > 0)
        {
            stats.y = newCommunication;
        }
        if (newAttitude > 0)
        {
            stats.z = newAttitude;
        }

        return stats;
    }
}
