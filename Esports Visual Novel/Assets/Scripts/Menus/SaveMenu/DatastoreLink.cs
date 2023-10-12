using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

/// <summary>
/// Used to transfer the DatastoreFlowchart data from one scene to the next when loading a new scene with the "LoadScene"
/// flowchart command. Otherwise, the data is lost because data isn't saved automatically.
/// </summary>
public class DatastoreLink : MonoBehaviour
{
    private FlowchartData flowchartData;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Call using a flowchart before transitioning to a new scene with the flowchart's LoadScene command. Super hack.
    /// </summary>
    public void LoadNextScene()
    {
        print("Encoded");
        Flowchart f = GameObject.Find("DatastoreFlowchart").GetComponent<Flowchart>();
        flowchartData = FlowchartData.Encode(f);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Scene Loaded");

        // If there isn't a SayDialog, we are on the Main Menu and should destroy the datastore.
        // If flowchartData is null, DatastoreLink doesn't need to do anything.
        if (GameObject.Find("SayDialog") == null || flowchartData == null)
        {
            Destroy(gameObject);
            return;
        }

        // If there is another datastore in the scene, replace its variables the encoded data.
        GameObject datastore = GameObject.FindGameObjectWithTag("Datastore");
        if (datastore != null)
        {
            print("Found another datastore");
            FlowchartData.Decode(flowchartData);
        }

        Destroy(gameObject);
    }
}
