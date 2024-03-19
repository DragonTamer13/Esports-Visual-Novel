using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SocialMediaController : MonoBehaviour
{
    private readonly string PostsCSVPrefix = "/Deets_";

    // Parent of created Post UI objects.
    [SerializeField] private GameObject postContainer;
    [SerializeField] private GameObject postPrefab;

    // TODO: Testing
    void Start()
    {
        SetSocialMediaPosts(PrepPhaseMenuController.MatchDay.Wednesday_2);
    }


    /// <summary>
    /// Reads and sets all the social media posts for a certain day.
    /// </summary>
    /// <param name="matchDay">The day to display social media posts results for.</param>
    public void SetSocialMediaPosts(PrepPhaseMenuController.MatchDay matchDay)
    {
        string postsCSVPath = Application.streamingAssetsPath + PostsCSVPrefix;

        switch (matchDay)
        {
            case PrepPhaseMenuController.MatchDay.Wednesday_2:
                postsCSVPath += "Wednesday2";
                break;
            default:
                Debug.LogError("No social media posts for " + matchDay.ToString());
                return;
        }

        using (StreamReader reader = File.OpenText(postsCSVPath + ".csv"))
        {
            int leftChar, rightChar = 0;
            string line = reader.ReadLine();
            string username = "";
            string message = "";
            string[] parsedEndOfLine = { }; // Contains the message attachment, redeets, and likes
            line = reader.ReadLine(); // Skip the first line because it's the header.

            while (line != null)
            {
                // Each line is: username (string, one word), message (string), attachment (string filepath), redeets (int), likes (int)
                leftChar = line.IndexOf(',');
                username = line.Substring(0, leftChar);

                // Parse the message, which might be in quotes
                leftChar++;
                if (line[leftChar] == '"')
                {
                    // Assumes the attachment doesn't have quotes. Gets more complicated if it does.
                    rightChar = line.LastIndexOf('"');
                }
                else
                {
                    rightChar = line.IndexOf(',', leftChar + 1);
                }
                message = line.Substring(leftChar, rightChar - leftChar);
                
                parsedEndOfLine = line.Substring(rightChar + 1).Split(',');

                // Implement attachments if we ever use them.
                if (parsedEndOfLine[0] != "")
                {
                    Debug.LogWarning("Social media post had an attachment: " + parsedEndOfLine[0]);
                }

                // Instantiate the new post prefab.
                GameObject newPost = Instantiate(postPrefab);
                newPost.transform.SetParent(postContainer.transform);
                newPost.transform.localScale = Vector3.one;
                if (parsedEndOfLine.Length < 3)
                {
                    newPost.GetComponent<Post>().SetText(username, message, parsedEndOfLine[1]);
                }
                else
                {
                    newPost.GetComponent<Post>().SetText(username, message, parsedEndOfLine[1], parsedEndOfLine[2]);
                }

                line = reader.ReadLine();
            }
        }
    }
}
