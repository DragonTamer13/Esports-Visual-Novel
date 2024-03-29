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
        [Tooltip("This day. Should only be a day that has a match.")]
        [SerializeField] protected PrepPhaseMenuController.MatchDay day;

        [Tooltip("The displayed title on the livestream UI.")]
        [SerializeField] protected StringData streamTitle;

        [Tooltip("Name of the player's team.")]
        [SerializeField] protected StringData playerTeam;

        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<LivestreamController>().Show(day, streamTitle, playerTeam);

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
