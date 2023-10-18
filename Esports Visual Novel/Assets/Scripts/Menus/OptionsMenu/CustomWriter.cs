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
        // true to have text continue automatically after a delay
        protected bool autoText = false;
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
        /// NOTE: Same as base function, except with additional code to continue automatically if auto is on.
        /// </summary>
        /// <param name="clear"></param>
        /// <returns></returns>
        protected override IEnumerator DoWaitForInput(bool clear)
        {
            NotifyPause();

            inputFlag = false;
            isWaitingForInput = true;

            // Continue after some time if auto is on
            if (autoText)
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
