using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Name text in the options menu preview.
    [SerializeField] private Text previewNameText;
    // Story text in the options menu preview.
    [SerializeField] private Text previewStoryText;

    private SayDialog previewSayDialog;
    private CustomWriter previewWriter;

    void Start()
    {
        previewSayDialog = GetComponentInChildren<SayDialog>();
        previewWriter = GetComponentInChildren<CustomWriter>();

        previewSayDialog.SetActive(true);
    }

    // Call when message speed option is changed.
    public void OnMessageSpeedChanged()
    {
        previewWriter.SetWritingSpeed(messageSpeedSlider.value);
        SayPreviewMessage();
    }

    // Call when font size option is changed.
    public void OnFontSizeChanged()
    {
        previewNameText.fontSize = nameFontSizes[fontSizeDropdown.value];
        previewStoryText.fontSize = storyFontSizes[fontSizeDropdown.value];
        SayPreviewMessage();
    }

    // Stop the preview running when the user exits the menu.
    public void OnBackButtonPressed()
    {
        if (previewWriter.IsWriting)
        {
            previewWriter.ResetWriter();
        }
        previewSayDialog.SetActive(false);
        gameObject.SetActive(false);
    }


    /// <summary>
    /// Use the preview SayDialog to show an example of the user's new options.
    /// </summary>
    protected void SayPreviewMessage()
    {
        if (!previewSayDialog.gameObject.activeInHierarchy)
        {
            previewSayDialog.SetActive(true);
            previewSayDialog.SetCharacter(previewCharacter);
        }

        previewSayDialog.StopAllCoroutines();
        previewSayDialog.Say(previewText, true, false, false, false, false, null, null);
    }
}
