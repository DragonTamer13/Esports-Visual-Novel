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
        [Tooltip("The click anywhere UI object")]
        [SerializeField] protected GameObject clickAnywhereGO;

        public void SwitchClickMode(bool canClickAnywhere)
        {
            /// <summary>
            /// Fungus's "ClickAnywhere" ClickMode doesn't work well with menus, so we enable/disable an invisible
            /// button that takes up most of the screen and continues the dialog if clicked.
            /// </summary>

            clickAnywhereGO.SetActive(canClickAnywhere);
        }

        /// <summary>
        /// Sets the button clicked flag ONLY if using ClickMode.ClickOnButton.
        /// </summary>
        public override void SetButtonClickedFlag()
        {
            // Only applies if we allow clicking on the button.
            if (clickMode == ClickMode.ClickOnButton)
            {
                SetNextLineFlag();
            }
        }
    }
}
