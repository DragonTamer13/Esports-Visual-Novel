using System.Collections;
using System.Collections.Generic;
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
    private readonly string[] Usernames = { "Name", "3749", "djskalfeh", "kjdahfDKJ", "89dddd2", "ThingThing", "IURnddkjlOI" };
    private readonly string[] Messages = { "Hello world!", "How are you today?", "What a good match.", "I love this game!!!" };

    // Parent object of chat messages
    [SerializeField] private Transform chat;
    // The prefab that is created for each chat message
    [SerializeField] private GameObject chatMessage;

    private List<TMP_Text> chatMessages; 
    private float curTime = 0.0f;
    private int currentChatMessageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        chatMessages = new List<TMP_Text>(chat.GetComponentsInChildren<TMP_Text>());
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime > 0.5f)
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
                                                     Usernames[Random.Range(0, Usernames.Length)] + "</color>: " + 
                                                     Messages[Random.Range(0, Messages.Length)];
        chatMessages[currentChatMessageIndex].transform.SetSiblingIndex(chatMessages.Count - 1);
        currentChatMessageIndex = (currentChatMessageIndex + 1) % chatMessages.Count;
    }
}
