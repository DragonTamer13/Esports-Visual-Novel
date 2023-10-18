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
        protected float autoDelayPerCharacter = 0.01f;
        protected DialogInput dialogInput;

        public void SetWritingSpeed(float newValue)
        {
            writingSpeed = newValue;
        }

        protected override void Awake()
        {
            base.Awake();

            dialogInput = GetComponent<DialogInput>();
        }

        /// <summary>
        /// Change dialog automatically progressing on/off.
        /// </summary>
        public void ToggleAutoText()
        {
            autoText = !autoText;
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
                // TODO: Calculate wait time. textAdapter.Text.Length * autoDelayPerCharacter
                StartCoroutine(AutoContinue(1.0f));
            }

            while (!inputFlag && !exitFlag)
            {
                yield return null;
            }

            isWaitingForInput = false;
            inputFlag = false;

            if (clear)
            {
                NotifyEnd(false);
                textAdapter.Text = "";
            }

            NotifyResume();
        }

        protected virtual IEnumerator AutoContinue(float waitTime)
        {
            // TODO: Stop the coroutine if we click to continue instead.
            // TODO: Continue automatically if auto is set after the text finishes printing on the screen.
            yield return new WaitForSeconds(waitTime);
            if (autoText)
            {
                dialogInput.SetNextLineFlag();
            }
        }
    }
}
