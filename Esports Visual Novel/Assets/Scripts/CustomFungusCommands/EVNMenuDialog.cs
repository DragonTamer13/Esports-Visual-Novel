using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// EVN extension of default Fungus MenuDialog
    /// </summary>
    public class EVNMenuDialog : MenuDialog
    {
        // Sound made when a menu button is pressed
        [SerializeField] private AudioClip buttonPressSound;

        // Since the MenuDialog disables itself when a button is pressed, play the button click sound elsewhere.
        public void PlayButtonClickSound()
        {
            FungusManager.Instance.MusicManager.PlaySound(buttonPressSound, FungusManager.Instance.MusicManager.GetSoundEffectVolume());
        }
    }
}
