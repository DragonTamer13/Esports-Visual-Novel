using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class Stats
    {
        public IntegerVariable MyIntVar;
    }

    /// <summary>
    /// A variable type containing the attribute data for all five characters in FFOD.
    /// NOTE: Couldn't get it to work, so variable is hidden from the flowchart "Add Variable" options.
    /// </summary>
    //[VariableInfo("FFOD", "Stats")]
    //[AddComponentMenu("")]
    //[System.Serializable]
    public class StatsVariable : VariableBase<Stats>
    {
        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return false;
        }
    }

    [System.Serializable]
    public struct StatsData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(StatsVariable))]
        public StatsVariable statsRef;

        [SerializeField]
        public Stats statsVal;

        public static implicit operator Stats(StatsData statsData)
        {
            return statsData.Value;
        }

        public StatsData(Stats v)
        {
            statsVal = v;
            statsRef = null;
        }

        public Stats Value
        {
            get { return (statsRef == null) ? statsVal : statsRef.Value; }
            set { if (statsRef == null) { statsVal = value; } else { statsRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (statsRef == null)
            {
                // TODO: IDK if it gives us a .ToString() by default or if I have to define it.
                return statsVal != null ? statsVal.ToString() : "Null";
            }
            else
            {
                return statsRef.Key;
            }
        }
    }
}
