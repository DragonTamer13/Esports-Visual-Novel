using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Set Scrim Results",
                 "Set which scrim results get shown for this day.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetScrimResults : Command
    {
        [Tooltip("This day")]
        [SerializeField] protected ScrimResultsMenu.MatchDay scrimResultsDay;

        #region Public members
        public override void OnEnter()
        {
            // TODO: Do this better
            FindObjectOfType<ScrimResultsMenu>().SetupScrimResults(scrimResultsDay);

            Continue();
        }

        public override string GetSummary()
        {
            return "";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
