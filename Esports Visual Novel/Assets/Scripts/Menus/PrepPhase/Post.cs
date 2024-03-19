using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Post : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text messageText;

    // Set the text of this Post
    public void SetText(string username, string text, string redeets = "0", string likes = "0")
    {
        usernameText.text = username;
        messageText.text = text;
    }
}
