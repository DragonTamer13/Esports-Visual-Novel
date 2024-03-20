﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "Set Up Prep Phase",
                 "Set what is shown on the prep phase for today.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetUpPrepPhase : Command
    {
        [Tooltip("This day")]
        [SerializeField] protected PrepPhaseMenuController.MatchDay day;

        [Tooltip("The player team's name")]
        [SerializeField] protected StringData teamName;

        #region Public members
        public override void OnEnter()
        {
            FindObjectOfType<PrepPhaseMenuController>().SetUpPrepPhase(day, teamName);

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
