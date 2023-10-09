using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fungus;

public class OptionsMenu : MonoBehaviour
{
    private const string previewText = "The quick brown fox jumps over the lazy dog.";
    private readonly int[] nameFontSizes = { 50, 60, 70 };
    private readonly int[] storyFontSizes = { 45, 50, 55 };

    // Slider for setting the text speed.
    [SerializeField] private Slider messageSpeedSlider;
    // Dropdown for font size options.
    [SerializeField] private TMP_Dropdown fontSizeDropdown;
    // Character object used to modify to preview display.
    [SerializeField] private Character previewCharacter;
    // The SayDialog object to instantiate to show a preview of the current options.
    [SerializeField] private GameObject optionsSayDialogPrefab;

    private SayDialog previewSayDialog;
    private CustomWriter previewWriter;
    private Text previewNameText;
    private Text previewStoryText;
    private CanvasGroup canvasGroup;
    private CustomWriter writer;
    private Text nameText;
    private Text storyText;

    private void Awake()
    {
        if (transform.parent != null && transform.parent.GetComponent<SaveMenu>() != null)
        {
            InitializeGameVariables();
        }

        previewSayDialog = GetComponentInChildren<SayDialog>();
        previewWriter = GetComponentInChildren<CustomWriter>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // If the Options Menu doesn't have a parent, then it comes from the Main Menu. Otherwise, it's from the in-game Save Menu.
        if (transform.parent == null)
        {
            GameObject.DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnGameLoadedFromMainMenu;
        }
        else
        {
            SceneManager.sceneLoaded += OnGameLoadedFromGame;
        }
    }

    /// <summary>
    /// Find objects in a game scene (not the Main Menu) used for key references.
    /// </summary>
    private void InitializeGameVariables()
    {
        if (GameObject.Find("SayDialog") == null)
        {
            print("TODO: Maybe fix why this is being called");
            return;
        }

        writer = GameObject.Find("SayDialog").GetComponent<CustomWriter>();
        nameText = GameObject.Find("SayDialog").transform.Find("Panel").Find("NameText").GetComponent<Text>();
        storyText = GameObject.Find("SayDialog").transform.Find("Panel").Find("StoryText").GetComponent<Text>();
    }

    /// <summary>
    /// Should only be called when the Options Menu on the Main Menu was loaded into the main game.
    /// Applies options set in the Main Menu to the relevant systems in the main game, then deletes 
    /// this instance of the Options Menu so we only have the one that's a child of the Save Menu. 
    /// Sort of a hack.
    /// </summary>
    public void OnGameLoadedFromMainMenu(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.Find("SaveMenu") != null)
        {
            GameObject.Find("SaveMenu").GetComponentInChildren<OptionsMenu>().SetAllOptions(
                messageSpeedSlider.value,
                fontSizeDropdown.value);
        }
        SceneManager.sceneLoaded -= OnGameLoadedFromMainMenu;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Called when the in-game Options Menu is loaded into another in-game scene as a child of the Save Menu.
    /// </summary>
    public void OnGameLoadedFromGame(Scene scene, LoadSceneMode mode)
    {
        InitializeGameVariables();
        SetAllOptions(messageSpeedSlider.value, fontSizeDropdown.value);
    }

    // Call when message speed option is changed.
    public void OnMessageSpeedChanged()
    {
        if (previewWriter != null)
        {
            previewWriter.SetWritingSpeed(messageSpeedSlider.value);
        }
        if (writer != null)
        {
            writer.SetWritingSpeed(messageSpeedSlider.value);
        }
        SayPreviewMessage();
    }

    // Call when font size option is changed.
    public void OnFontSizeChanged()
    {
        if (previewNameText != null)
        {
            previewNameText.fontSize = nameFontSizes[fontSizeDropdown.value];
            previewStoryText.fontSize = storyFontSizes[fontSizeDropdown.value];
        }
        if (nameText != null && storyText != null)
        {
            nameText.fontSize = nameFontSizes[fontSizeDropdown.value];
            storyText.fontSize = storyFontSizes[fontSizeDropdown.value];
        }
        SayPreviewMessage();
    }

    /// <summary>
    /// Show the Options menu
    /// 
    /// NOTE: How this should be done:
    /// - Load options from disk when opening the menu, and set UI elements to match.
    /// - Write options to disk when closing the menu, and set the in-game SayDialog properties to match.
    /// - When loading a scene with a SayDialog, read options from disk and set SayDialog properties to match.
    /// - The Main Menu Options Menu shouldn't be DontDestroyOnLoad. Instead, just use the saved options from disk
    ///   to set the in-game properties.
    /// - It should be fine if the in-game options menu is DontDestroyOnLoad as a child of the Save Menu, just unbind 
    ///   the OnSceneLoaded functionality.
    /// </summary>
    public void OnShow()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Create the preview SayDialog when opening the menu. This is to prevent Fungus from writing actual story text to
        // the Options Menu preview.
        GameObject previewGO = GameObject.Instantiate(optionsSayDialogPrefab);
        //previewGO.transform.SetParent(transform, false);
        previewSayDialog = previewGO.GetComponent<SayDialog>();
        previewWriter = previewGO.GetComponent<CustomWriter>();
        previewNameText = previewGO.transform.Find("Panel").Find("NameText").GetComponent<Text>();
        previewStoryText = previewGO.transform.Find("Panel").Find("StoryText").GetComponent<Text>();
        previewGO.GetComponent<Canvas>().sortingOrder = GetComponent<Canvas>().sortingOrder + 1;
    }

    // Stop the preview running when the user exits the menu.
    public void OnBackButtonPressed()
    {
        if (previewWriter.IsWriting)
        {
            previewWriter.ResetWriter();
        }
        Destroy(previewSayDialog.gameObject);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Set the options in the menu and in the game to be consistent with the values of the Options Menu on
    /// the Main Menu.
    /// </summary>
    public void SetAllOptions(float messageSpeed, int fontSize)
    {
        messageSpeedSlider.value = messageSpeed;
        fontSizeDropdown.value = fontSize;

        writer.SetWritingSpeed(messageSpeedSlider.value);
        nameText.fontSize = nameFontSizes[fontSizeDropdown.value];
        storyText.fontSize = storyFontSizes[fontSizeDropdown.value];
    }


    /// <summary>
    /// Use the preview SayDialog to show an example of the user's new options.
    /// </summary>
    protected void SayPreviewMessage()
    {
        // Don't do anything if the menu isn't turned on.
        if (canvasGroup.alpha < 1.0)
        {
            return;
        }

        if (!previewSayDialog.gameObject.activeInHierarchy)
        {
            previewSayDialog.SetActive(true);
            previewSayDialog.SetCharacter(previewCharacter);
        }

        previewSayDialog.StopAllCoroutines();
        // TODO: Calling this adds to the Transcript menu, filling it with the sample text and the current in-game story text.
        //       Maybe find some way to fix this by overriding Say(). Would need to make a custom SayDialogue script too then...
        previewSayDialog.Say(previewText, true, false, false, false, false, null, null);
    }
}
