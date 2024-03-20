using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Livestream POG",
                 "POGGERS in the chat.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LivestreamPog : Command
    {
        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<LivestreamController>().Pog();

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
