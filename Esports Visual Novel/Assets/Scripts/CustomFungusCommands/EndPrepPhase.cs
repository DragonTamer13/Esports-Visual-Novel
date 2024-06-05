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
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevAtroposStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("AtroposStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevBoigaStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("BoigaStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevHajoonStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("HajoonStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevMaedayStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("MaedayStats"));
            datastoreFlowchart.GetVariable<Vector4Variable>("PrevVelocityStats").Apply(SetOperator.Assign,
                datastoreFlowchart.GetVariable<Vector4Variable>("VelocityStats"));

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
