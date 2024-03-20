using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Livestream LUL",
                 "LULs in the chat.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LivestreamLul : Command
    {
        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<LivestreamController>().Lul();

            Continue();
        }

        public override string GetSummary()
        {
            return "";
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 204, 142, 255);
        }

        #endregion
    }
}
