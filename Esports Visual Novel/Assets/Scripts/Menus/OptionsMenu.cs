using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class OptionsMenu : MonoBehaviour
{
    private const string previewText = "The quick brown fox jumps over the lazy dog.";

    // Slider for setting the text speed.
    [SerializeField] private Slider messageSpeedSlider;
    // Character object used to modify to preview display.
    [SerializeField] private Character previewCharacter;

    private SayDialog previewSayDialog;
    private CustomWriter previewWriter;

    void Start()
    {
        previewSayDialog = GetComponentInChildren<SayDialog>();
        previewWriter = GetComponentInChildren<CustomWriter>();

        previewSayDialog.SetActive(true);

        Parent p = GetComponent<Parent>();
        if (p != null)
        {
            p.Function();
        }
    }

    public void OnMessageSpeedChanged()
    {
        previewWriter.SetWritingSpeed(messageSpeedSlider.value);
        SayPreviewMessage();
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
