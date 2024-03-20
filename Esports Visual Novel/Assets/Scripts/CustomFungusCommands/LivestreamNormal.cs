using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Livestream Normal",
                 "Regular chat behavior.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LivestreamNormal : Command
    {
        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<LivestreamController>().Normal();

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
