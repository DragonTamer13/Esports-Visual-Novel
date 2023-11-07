using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Come with extra functionality for changing protected properties.
    /// </summary>
    public class CustomDialogInput : DialogInput
    {
        public void SetClickMode(ClickMode newMode)
        {
            clickMode = newMode;
        }
    }
}
