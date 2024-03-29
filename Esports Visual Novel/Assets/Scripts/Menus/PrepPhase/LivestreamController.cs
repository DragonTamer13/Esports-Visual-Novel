using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simulates a livestream video and chatbox.
/// </summary>
public class LivestreamController : MonoBehaviour
{
    private readonly string[] Colors = { "red", "blue", "#008000", "#B22222", "#FF7F50", "#9ACD32", "#FF4500", "#2E8B57", 
                                         "#DAA520", "#D2691E", "#5F9EA0", "#1E90FF", "#FF69B4", "#8A2BE2", "#00FF7F" };
    // Special string indicating to insert the player team's name into some text.
    private readonly string PlayerTeamNameKey = "[PlayerTeam]";
    // Time between messages for the different chat states.
    private const float NormalMessageTime = 0.25f;
    private const float PogMessageTime = 0.05f;
    private const float LulMessageTime = 0.10f;

    private enum ChatState
    {
        Normal,
        Pog,
        Lul
    }

    // Parent object of chat messages
    [SerializeField] private Transform chat;
    // The title text object.
    [SerializeField] private TMP_Text streamTitleText;
    // The prefab that is created for each chat message
    [SerializeField] private GameObject chatMessage;

    private List<TMP_Text> chatMessages; 
    private float curTime = 0.0f;
    private float maxTime = 0.0f; // How long until a new message is posted.
    private int currentChatMessageIndex = 0;
    private List<string> usernames = new List<string>();
    private List<string> messages = new List<string>();
    private CanvasGroup canvasGroup;
    // Determines the type and frequences of chat messages being posted.
    private ChatState state = ChatState.Normal;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        chatMessages = new List<TMP_Text>(chat.GetComponentsInChildren<TMP_Text>());
        Normal();
        Hide();

        // Read the chat usernames from a CSV file.
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/MinMax_Usernames.csv"))
        {
            string line = reader.ReadLine();
            line = reader.ReadLine();

            while (line != null)
            {
                if (line != "")
                {
                    usernames.Add(line);
                }

                line = reader.ReadLine();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha < 1.0f)
        {
            return;
        }

        if (curTime > maxTime)
        {
            CreateChatMessage();
            curTime = 0.0f;
        }
        else
        {
            curTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Show this menu.
    /// </summary>
    /// <param name="day">Which CSV to read for the chat messages for this day. Value should have a corresponding chat messages CSV.</param>
    /// <param name="streamTitle">What is shown on the screen for the title of this stream.</param>
    /// <param name="playerTeam">The chosen name for the player's team.</param>
    public void Show(PrepPhaseMenuController.MatchDay day, string streamTitle, string playerTeam)
    {
        // Read the chat messages for this match from a CSV file.
        string fileToOpen = Application.streamingAssetsPath + "/MinMax_";
        switch (day)
        {
            case PrepPhaseMenuController.MatchDay.Friday_4_vs_3:
                fileToOpen += "Friday4vs3";
                break;
            default:
                break;
        }
        fileToOpen += ".csv";

        using (StreamReader reader = File.OpenText(fileToOpen))
        {
            string message = "";
            string line = reader.ReadLine();
            line = reader.ReadLine();   // Skip the header line

            while (line != null)
            {
                message = line;
                // Remove extra characters from the message while keeping its formatting.
                if (message.IndexOf('"') >= 0)
                {
                    // The message is in quotes.
                    int messageEnd = message.LastIndexOf("\"");
                    if (messageEnd == message.Length - 1)
                    {
                        message = message.Substring(1, messageEnd - 1);
                    }
                    else
                    {
                        // The message spans multiple lines
                        message = message.Substring(1);
                        while (line != null)
                        {
                            message += "\n";
                            line = reader.ReadLine();
                            messageEnd = line.LastIndexOf("\"");

                            if (messageEnd > 0)
                            {
                                message += line.Substring(0, messageEnd - 1);
                                break;
                            }
                            else
                            {
                                message += line;
                            }
                        }
                    }
                    // Reading in a CSV will change quotes in the original text into double quotes, so change those back.
                    message = message.Replace("\"\"", "\"");
                }

                message.Replace(PlayerTeamNameKey, playerTeam);

                if (message != "")
                {
                    messages.Add(message);
                }

                line = reader.ReadLine();
            }
        }

        // Make the livestream visible
        streamTitleText.text = streamTitle.Replace(PlayerTeamNameKey, playerTeam);
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide this menu.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Display regular chat messages.
    /// </summary>
    public void Normal()
    {
        state = ChatState.Normal;
        maxTime = NormalMessageTime;
    }

    /// <summary>
    /// Display POGs at an increased frequency.
    /// </summary>
    public void Pog()
    {
        state = ChatState.Pog;
        maxTime = PogMessageTime;
    }

    /// <summary>
    /// Display LULs at an increased frequency.
    /// </summary>
    public void Lul()
    {
        state = ChatState.Lul;
        maxTime = LulMessageTime;
    }

    /// <summary>
    /// Randomly generates a new chat message to display in the chatbox.
    /// </summary>
    private void CreateChatMessage()
    {
        chatMessages[currentChatMessageIndex].text = "<color=" + Colors[Random.Range(0, Colors.Length)] + ">" +
                                                             usernames[Random.Range(0, usernames.Count)] + "</color>: ";
        switch (state)
        {
            case ChatState.Normal:
                chatMessages[currentChatMessageIndex].text += messages[Random.Range(0, messages.Count)];
                break;
            case ChatState.Pog:
                chatMessages[currentChatMessageIndex].text += "<sprite=\"POGGERS\" name=\"POGGERS\">";
                break;
            case ChatState.Lul:
                chatMessages[currentChatMessageIndex].text += "<sprite=\"LUL\" name=\"LUL\">";
                break;
            default:
                Debug.LogWarning("No Livestream chat functionality for state " + state.ToString());
                break;
        }

        Canvas.ForceUpdateCanvases();
        chatMessages[currentChatMessageIndex].transform.SetSiblingIndex(chatMessages.Count - 1);
        currentChatMessageIndex = (currentChatMessageIndex + 1) % chatMessages.Count;
    }
}
