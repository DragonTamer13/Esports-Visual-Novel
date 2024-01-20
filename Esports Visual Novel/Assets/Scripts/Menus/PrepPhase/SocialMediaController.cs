using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SocialMediaController : MonoBehaviour
{
    // Parent of created Post UI objects.
    [SerializeField] private GameObject postContainer;
    [SerializeField] private GameObject postPrefab;

    /// <summary>
    /// Create a new post on the social media screen. Intended to be called from a flowchart.
    /// </summary>
    /// <param name="text">First "word" is the poster name, rest is the post content.</param>
    public void CreatePost(string text)
    {
        int parseIndex = text.IndexOf(' ');

        if (parseIndex < 0 || text.Length <= 0)
        {
            return;
        }

        GameObject newPost = Instantiate(postPrefab);

        newPost.transform.SetParent(postContainer.transform);
        newPost.transform.localScale = Vector3.one;
        newPost.GetComponent<Post>().SetText(text.Substring(0, parseIndex), text.Substring(parseIndex + 1));
    }
}
