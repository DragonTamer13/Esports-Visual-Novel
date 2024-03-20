using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Show Livestream",
                 "Show the livestream background.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ShowLivestream : Command
    {
        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<LivestreamController>().Show();

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
