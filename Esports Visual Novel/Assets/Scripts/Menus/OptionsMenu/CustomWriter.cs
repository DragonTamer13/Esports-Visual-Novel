using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Same as the default Fungus Writer, except you can call SetWritingSpeed to change the writing speed.
    /// </summary>
    public class CustomWriter : Writer
    {
        // True to have text continue automatically after a delay
        protected bool autoText = false;
        protected bool skipping = false;
        // How long to delay auto continuing per character in the most recent story text.
        protected float autoDelayPerCharacter = 0.03f;
        protected DialogInput dialogInput;
        protected Coroutine autoCoroutine;

        public void SetWritingSpeed(float newValue)
        {
            writingSpeed = newValue;
        }

        public void SetAutoDelay(float newValue)
        {
            autoDelayPerCharacter = newValue;
        }

        protected override void Awake()
        {
            base.Awake();

            dialogInput = GetComponent<DialogInput>();
        }

        /// <summary>
        /// Change dialog automatically progressing on/off. Continue immediately if we have finished writing.
        /// </summary>
        public void ToggleAutoText()
        {
            autoText = !autoText;

            if (autoText && isWaitingForInput)
            {
                dialogInput.SetNextLineFlag();
            }
            else if (!autoText && autoCoroutine != null)
            {
                StopCoroutine(autoCoroutine);
            }
        }

        /// <summary>
        /// Quickly progress through dialogue until a menu shows up, the scene changes, or dialogue ends.
        /// </summary>
        public void ToggleSkip()
        {
            skipping = !skipping;

            // Advance the dialogue line that we're on right now since skipping only gets checked at the start of a line.
            if (skipping)
            {
                dialogInput.SetNextLineFlag();
            }

            /*
             * NOTE: dialogue advancement code flow:
             * - When you click, DialogInput.SetNextLineFlag() is called.
             * - DialogInput.SetNextLineFlag() sets DialogInput.nextLineInputFlag
             * - DialogInput.nextLineInputFlag calls Writer.OnNextLineEvent() 
             * - Writer.OnNextLineEvent() sets Writer.inputFlag = true and notifies other input listeners.
             * - Writer.inputFlag = true causes the text to instantly finish writing OR for the next line to show up
             */
        }

        /// <summary>
        /// Writes text using a typewriter effect to a UI text object. Skips writing if in skip mode.
        /// </summary>
        /// <param name="content">Text to be written</param>
        /// <param name="clear">If true clears the previous text.</param>
        /// <param name="waitForInput">Writes the text and then waits for player input before calling onComplete.</param>
        /// <param name="stopAudio">Stops any currently playing audioclip.</param>
        /// <param name="waitForVO">Wait for the Voice over to complete before proceeding</param>
        /// <param name="audioClip">Audio clip to play when text starts writing.</param>
        /// <param name="onComplete">Callback to call when writing is finished.</param>
        public override IEnumerator Write(string content, bool clear, bool waitForInput, bool stopAudio, bool waitForVO, AudioClip audioClip, System.Action onComplete)
        {
            Coroutine result = StartCoroutine(base.Write(content, clear, waitForInput, stopAudio, waitForVO, audioClip, onComplete));

            if (skipping)
            {
                dialogInput.SetNextLineFlag();
            }

            yield return result;
        }

        /// <summary>
        /// Resets Writer variables. Call when disabling the Writer before it finishes displaying a line.
        /// Taken from end of Writer.ProcessTokens(), which doesn't get called if the Writer is disabled 
        /// before it finishes writing.
        /// </summary>
        public void ResetWriter()
        {
            inputFlag = false;
            exitFlag = false;
            isWaitingForInput = false;
            isWriting = false;
        }

        /// <summary>
        /// NOTE: Same as base function, except with additional code for "auto" and "skip" modes.
        /// </summary>
        /// <param name="clear"></param>
        /// <returns></returns>
        protected override IEnumerator DoWaitForInput(bool clear)
        {
            NotifyPause();

            inputFlag = false;
            isWaitingForInput = true;

            if (skipping)
            {
                dialogInput.SetNextLineFlag();
            }
            // Continue after some time if auto is on
            else if (autoText)
            {
                autoCoroutine = StartCoroutine(AutoContinue(textAdapter.Text.Length * autoDelayPerCharacter));
            }

            while (!inputFlag && !exitFlag)
            {
                yield return null;
            }

            isWaitingForInput = false;
            inputFlag = false;

            // Prevent automatic continue if the mouse was clicked instead.
            if (autoCoroutine != null)
            {
                StopCoroutine(autoCoroutine);
            }

            if (clear)
            {
                NotifyEnd(false);
                textAdapter.Text = "";
            }

            NotifyResume();
        }

        protected virtual IEnumerator AutoContinue(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (autoText)
            {
                dialogInput.SetNextLineFlag();
            }
        }
    }
}
