using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Set Partial Stats",
                 "Sets some of the values of a stat Vector4 instead of all of them. ")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetPartialStats : Command
    {
        [Tooltip("Vector4 variable containing a player's stats")]
        [SerializeField] protected Vector4Data StatsVector;

        [Tooltip("The player team's name")]
        [SerializeField] protected StringData teamName;

        #region Public members
        public override void OnEnter()
        {

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
