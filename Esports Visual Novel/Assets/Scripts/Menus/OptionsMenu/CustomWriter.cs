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
        public void SetWritingSpeed(float newValue)
        {
            writingSpeed = newValue;
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
    }
}
