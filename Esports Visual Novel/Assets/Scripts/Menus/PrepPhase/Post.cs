using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Post : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text likesText;
    [SerializeField] private TMP_Text redeetsText;
    [SerializeField] private GameObject likedImage;
    [SerializeField] private GameObject redeetedImage;

    // Set the text of this Post
    public void SetText(string username, string text, string redeets = "0", string likes = "0")
    {
        if (redeets == "")
        {
            redeets = "0";
        }
        if (likes == "")
        {
            likes = "0";
        }

        usernameText.text = username;
        messageText.text = text;
        likesText.text = likes;
        redeetsText.text = redeets;
    }

    /// <summary>
    /// Updates "Like" UI elements
    /// </summary>
    public void ToggleLike()
    {
        likedImage.SetActive(!likedImage.activeInHierarchy);
        if (likedImage.activeInHierarchy)
        {
            likesText.text = (int.Parse(likesText.text) + 1).ToString();
        }
        else
        {
            likesText.text = (int.Parse(likesText.text) - 1).ToString();
        }
    }

    /// <summary>
    /// Updates "Redeet" UI elements
    /// </summary>
    public void ToggleRedeet()
    {
        redeetedImage.SetActive(!redeetedImage.activeInHierarchy);
        if (redeetedImage.activeInHierarchy)
        {
            redeetsText.text = (int.Parse(redeetsText.text) + 1).ToString();
        }
        else
        {
            redeetsText.text = (int.Parse(redeetsText.text) - 1).ToString();
        }
    }
}
