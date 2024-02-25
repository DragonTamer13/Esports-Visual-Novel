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
    //private readonly string[] Usernames = { "Name", "3749", "djskalfeh", "kjdahfDKJ", "89dddd2", "ThingThing", "IURnddkjlOI" };
    //private readonly string[] Messages = { "Hello world!", "How are you today?", "What a good match.", "I love this game!!!" };

    // Parent object of chat messages
    [SerializeField] private Transform chat;
    // The prefab that is created for each chat message
    [SerializeField] private GameObject chatMessage;

    private List<TMP_Text> chatMessages; 
    private float curTime = 0.0f;
    private int currentChatMessageIndex = 0;
    private List<string> usernames = new List<string>();
    private List<string> messages = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        chatMessages = new List<TMP_Text>(chat.GetComponentsInChildren<TMP_Text>());

        using (StreamReader reader = File.OpenText(Application.dataPath + "/Resources/LivestreamMessages.csv"))
        {
            int deliminator = 0;
            string username = "";
            string message = "";
            string line = reader.ReadLine();
            line = reader.ReadLine();

            while (line != null)
            {
                deliminator = line.IndexOf(',');
                if (deliminator >= 0)
                {
                    username = line.Substring(0, deliminator);
                    message = line.Substring(deliminator + 1);
                    if (username != "")
                    {
                        usernames.Add(username);
                    }

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
                            // TODO: improve condition
                            while (line != null)
                            {
                                message += "\n";
                                line = reader.ReadLine();
                                messageEnd = line.LastIndexOf("\"");

                                if (messageEnd >= 0)
                                {
                                    message += line.Substring(0, messageEnd-1);
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

                    if (message != "")
                    {
                        messages.Add(message);
                    }
                }

                line = reader.ReadLine();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime > 0.25f)
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
    /// Randomly generates a new chat message to display in the chatbox.
    /// </summary>
    private void CreateChatMessage()
    {
        chatMessages[currentChatMessageIndex].text = "<color=" + Colors[Random.Range(0, Colors.Length)] + ">" + 
                                                     usernames[Random.Range(0, usernames.Count)] + "</color>: " + 
                                                     messages[Random.Range(0, messages.Count)];
        chatMessages[currentChatMessageIndex].transform.SetSiblingIndex(chatMessages.Count - 1);
        currentChatMessageIndex = (currentChatMessageIndex + 1) % chatMessages.Count;
    }
}
