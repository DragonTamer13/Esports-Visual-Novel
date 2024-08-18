using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("EVN",
                 "End Prep Phase",
                 "Record new previous stat values and the end of the prep phase.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class EndPrepPhase : Command
    {
        #region Public members
        public override void OnEnter()
        {
            Flowchart datastoreFlowchart = GameObject.Find("DatastoreFlowchart").GetComponent<Flowchart>();

            // Set the recorded previous day's stat values to the stat values at the start of this day's prep phase.
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevAtroposStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrepAtroposStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevBoigaStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrepBoigaStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevHajoonStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrepHajoonStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevMaedayStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrepMaedayStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevVelocityStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("StartOfPrepVelocityStats"));

            // Set the current day's notes to the next day's notes. Notes will not be read again until the next day,
            // so it shouldn't matter if the current/next day's notes are off.
            datastoreFlowchart.GetVariable<StringVariable>("AtroposNotes").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<StringVariable>("NextAtroposNotes"));
            datastoreFlowchart.GetVariable<StringVariable>("NextAtroposNotes").Apply(SetOperator.Assign, "");
            datastoreFlowchart.GetVariable<StringVariable>("BoigaNotes").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<StringVariable>("NextBoigaNotes"));
            datastoreFlowchart.GetVariable<StringVariable>("NextBoigaNotes").Apply(SetOperator.Assign, "");
            datastoreFlowchart.GetVariable<StringVariable>("HajoonNotes").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<StringVariable>("NextHajoonNotes"));
            datastoreFlowchart.GetVariable<StringVariable>("NextHajoonNotes").Apply(SetOperator.Assign, "");
            datastoreFlowchart.GetVariable<StringVariable>("MaedayNotes").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<StringVariable>("NextMaedayNotes"));
            datastoreFlowchart.GetVariable<StringVariable>("NextMaedayNotes").Apply(SetOperator.Assign, "");
            datastoreFlowchart.GetVariable<StringVariable>("VelocityNotes").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<StringVariable>("NextVelocityNotes"));
            datastoreFlowchart.GetVariable<StringVariable>("NextVelocityNotes").Apply(SetOperator.Assign, "");

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
