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
    // Parent object of chat messages
    [SerializeField] private Transform chat;
    // The prefab that is created for each chat message
    [SerializeField] private GameObject chatMessage;
    [SerializeField] private ScrollRect scroll;

    private float curTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (curTime > 1.0f)
        {
            GameObject newChatMessage = GameObject.Instantiate(chatMessage, chat);
            newChatMessage.GetComponent<TMP_Text>().text = "<color=red>Username</color>: Hello world!";
            curTime = 0.0f;
        }
        else
        {
            curTime += Time.deltaTime;
            // Force the latest message to always be fully visible in the chat window.
            scroll.verticalNormalizedPosition = 0.0f;
        }
    }
}
