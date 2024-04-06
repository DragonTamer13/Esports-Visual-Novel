using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Fungus;

public class OptionsMenu : MonoBehaviour
{
    private const string previewText = "The quick brown fox jumps over the lazy dog.";
    private readonly FullScreenMode[] fullScreenModes = { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
    private readonly int[] resolutionWidth  = { 1024, 1152, 1280, 1280, 1280, 1280, 1360, 1440, 1440, 1600, 1600, 1680, 1920, 1920, 2560, 2560, 3200, 3840, 3840 };
    private readonly int[] resolutionHeight = { 768,  864,  720,  800,  960,  1024, 768,  900,  1080, 900,  1024, 1050, 1080, 1200, 1440, 1600, 1800, 2160, 2400 };
    private readonly int[] nameFontSizes = { 50, 60, 70 };
    private readonly int[] storyFontSizes = { 45, 50, 55 };

    // The keys for settings stored in PlayerPrefs
    private const string DisplayModeKey = "DisplayMode";
    private const string ResolutionKey = "Resolution";
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MessageSpeedKey = "MessageSpeed";
    private const string AutoDelayKey = "AutoDelay";
    private const string FontSizeKey = "FontSize";
    private const string ContinueModeKey = "ContinueMode";

    // Dropdown for screen mode options.
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    // Dropdown for resolution options.
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    // Slider for setting the text speed.
    [SerializeField] private Slider masterVolumeSlider;
    // Slider for setting the text speed.
    [SerializeField] private Slider musicVolumeSlider;
    // Slider for setting the text speed.
    [SerializeField] private Slider sfxVolumeSlider;
    // Slider for setting the text speed.
    [SerializeField] private Slider messageSpeedSlider;
    // Slider for setting the auto continue speed.
    [SerializeField] private Slider autoDelaySlider;
    // Dropdown for font size options.
    [SerializeField] private TMP_Dropdown fontSizeDropdown;
    // Dropdown for continue mode options.
    [SerializeField] private TMP_Dropdown continueModeDropdown;
    // Character object used to modify to preview display.
    [SerializeField] private GameObject previewCharacterPrefab;
    // The SayDialog object to instantiate to show a preview of the current options.
    [SerializeField] private GameObject optionsSayDialogPrefab;
    // The main game audio mixer.
    [SerializeField] private AudioMixer audioMixer;

    private SayDialog previewSayDialog;
    private CustomWriter previewWriter;
    private Text previewNameText;
    private Text previewStoryText;
    private CanvasGroup canvasGroup;
    private CustomWriter writer;
    private CustomDialogInput dialogInput;
    private Text nameText;
    private Text storyText;
    private Character previewCharacter;

    private void Awake()
    {
        previewSayDialog = GetComponentInChildren<SayDialog>();
        previewWriter = GetComponentInChildren<CustomWriter>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
        // Manually call OnSceneLoaded in Start because you can't set AudioMixer properties in Awake.
        OnSceneLoaded();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Find objects in a game scene (not the Main Menu) used for key references.
    /// </summary>
    private void InitializeGameVariables()
    {
        GameObject sayDialog = GameObject.Find("SayDialog");

        if (sayDialog == null)
        {
            Debug.LogError("TODO: Maybe fix why this is being called");
            return;
        }

        writer = sayDialog.GetComponent<CustomWriter>();
        nameText = sayDialog.transform.Find("Panel").Find("Nametag").Find("NameText").GetComponent<Text>();
        storyText = sayDialog.transform.Find("Panel").Find("StoryText").GetComponent<Text>();
        dialogInput = sayDialog.GetComponent<CustomDialogInput>();
    }

    /// <summary>
    /// Returns the index of the option in resolutionDropdown that matches the system's screen resolution, or 0 if no match is found.
    /// </summary>
    private int GetDefaultResolutionSetting()
    {
        int w = Screen.currentResolution.width;
        int h = Screen.currentResolution.height;

        for (int i = 0; i < resolutionWidth.Length; i++)
        {
            if (resolutionWidth[i] == w && resolutionHeight[i] == h)
            {
                return i;
            }
        }

        return 0;
    }

    /// <summary>
    /// Set the SayDialog properties when we load into a new scene. Set references to key SayDialog components if 
    /// they exist in the scene.
    /// </summary>
    public void OnSceneLoaded(Scene scene=new Scene(), LoadSceneMode mode=LoadSceneMode.Single)
    {
        if (transform.parent != null && transform.parent.GetComponent<SaveMenu>() != null)
        {
            InitializeGameVariables();
        }

        ApplyOptions();
    }

    public void OnDisplayModeChanged()
    {
        Screen.SetResolution(resolutionWidth[resolutionDropdown.value], resolutionHeight[resolutionDropdown.value], fullScreenModes[displayModeDropdown.value]);
    }

    public void OnResolutionChanged()
    {
        Screen.SetResolution(resolutionWidth[resolutionDropdown.value], resolutionHeight[resolutionDropdown.value], fullScreenModes[displayModeDropdown.value]);
    }

    // Call after changing the master volume option.
    public void OnMasterVolumeChanged()
    {
        audioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
        SayPreviewMessage();
    }

    // Call after changing the music volume option.
    public void OnMusicVolumeChanged()
    {
        audioMixer.SetFloat("MusicVolume", musicVolumeSlider.value);
        SayPreviewMessage();
    }

    // Call after changing the SFX volume option.
    public void OnSFXVolumeChanged()
    {
        audioMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);
        SayPreviewMessage();
    }

    // Call when message speed option is changed.
    public void OnMessageSpeedChanged()
    {
        if (previewWriter != null)
        {
            previewWriter.SetWritingSpeed(messageSpeedSlider.value);
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
        else
        {
            print("Couldn't change preview font size");
        }
        SayPreviewMessage();
    }

    /// <summary>
    /// Show the Options menu
    /// </summary>
    public void OnShow()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Create the preview SayDialog when opening the menu. This is to prevent Fungus from writing actual story text to
        // the Options Menu preview.
        GameObject previewGO = GameObject.Instantiate(optionsSayDialogPrefab);
        previewSayDialog = previewGO.GetComponent<SayDialog>();
        previewWriter = previewGO.GetComponent<CustomWriter>();
        previewNameText = previewGO.transform.Find("Panel").Find("NameText").GetComponent<Text>();
        previewStoryText = previewGO.transform.Find("Panel").Find("StoryText").GetComponent<Text>();
        previewGO.GetComponent<Canvas>().sortingOrder = GetComponent<Canvas>().sortingOrder + 1;

        // Load user settings and update Options Menu UI to match.
        displayModeDropdown.value = PlayerPrefs.GetInt(DisplayModeKey, displayModeDropdown.value);
        resolutionDropdown.value = PlayerPrefs.GetInt(ResolutionKey, GetDefaultResolutionSetting());
        masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, masterVolumeSlider.value);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, musicVolumeSlider.value);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, sfxVolumeSlider.value);
        messageSpeedSlider.value = PlayerPrefs.GetFloat(MessageSpeedKey, messageSpeedSlider.value);
        autoDelaySlider.value = PlayerPrefs.GetFloat(AutoDelayKey, autoDelaySlider.value);
        fontSizeDropdown.value = PlayerPrefs.GetInt(FontSizeKey, fontSizeDropdown.value);
        continueModeDropdown.value = PlayerPrefs.GetInt(ContinueModeKey, continueModeDropdown.value);
    }

    // Stop the preview running when the user exits the menu.
    public void OnBackButtonPressed()
    {
        if (previewWriter.IsWriting)
        {
            previewWriter.ResetWriter();
        }
        // Preview objects cause problems if they're DontDestroyOnLoad, so destroy them when closing the Menu.
        Destroy(previewSayDialog.gameObject);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Save options to PlayerPrefs
        PlayerPrefs.SetInt(DisplayModeKey, displayModeDropdown.value);
        PlayerPrefs.SetInt(ResolutionKey, resolutionDropdown.value);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolumeSlider.value);
        PlayerPrefs.SetFloat(MessageSpeedKey, messageSpeedSlider.value);
        PlayerPrefs.SetFloat(AutoDelayKey, autoDelaySlider.value);
        PlayerPrefs.SetInt(FontSizeKey, fontSizeDropdown.value);
        PlayerPrefs.SetInt(ContinueModeKey, continueModeDropdown.value);
        PlayerPrefs.Save();

        ApplyOptions();
    }

    /// <summary>
    /// Apply the user's options to the SayDialog if one exists in the scene. Should be called after the options could have been
    /// changed, such as when closing the Options Menu or when loading into a new scene.
    /// </summary>
    public void ApplyOptions()
    {
        // There isn't a writer in this scene, so there isn't a SayDialog. Don't try changing any settings.
        if (writer == null)
        {
            return;
        }

        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat(MasterVolumeKey, masterVolumeSlider.value));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat(MusicVolumeKey, musicVolumeSlider.value));
        audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat(SFXVolumeKey, sfxVolumeSlider.value));
        writer.SetWritingSpeed(PlayerPrefs.GetFloat(MessageSpeedKey, messageSpeedSlider.value));
        writer.SetAutoDelay(PlayerPrefs.GetFloat(AutoDelayKey, autoDelaySlider.value));
        nameText.fontSize = nameFontSizes[PlayerPrefs.GetInt(FontSizeKey, fontSizeDropdown.value)];
        storyText.fontSize = storyFontSizes[PlayerPrefs.GetInt(FontSizeKey, fontSizeDropdown.value)];
        dialogInput.SwitchClickMode(PlayerPrefs.GetInt(ContinueModeKey, continueModeDropdown.value) == 0);
    }

    /// <summary>
    /// Set the options in the menu and in the game to be consistent with the values of the Options Menu on
    /// the Main Menu.
    /// TODO: See if still needed
    /// </summary>
    public void SetAllOptions(float messageSpeed, int fontSize)
    {
        Debug.LogError("SetAllOptions called. Might be obsolete.");
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
            if (previewCharacter == null)
            {
                GameObject preview = Instantiate(previewCharacterPrefab);
                preview.transform.SetParent(transform, false);
                previewCharacter = preview.GetComponent<Character>();
            }
            previewSayDialog.SetCharacter(previewCharacter);
        }

        previewSayDialog.StopAllCoroutines();
        // TODO: Calling this adds to the Transcript menu, filling it with the sample text and the current in-game story text.
        //       Maybe find some way to fix this by overriding Say(). Would need to make a custom SayDialogue script too then...
        previewSayDialog.Say(previewText, true, false, false, false, false, null, null);
    }
}
