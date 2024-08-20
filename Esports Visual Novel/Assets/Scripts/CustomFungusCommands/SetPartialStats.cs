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
        // Names of the main characters of EVN. Used as prefixes for character-specific variables.
        public enum CharacterNames
        {
            Atropos,
            Boiga,
            Hajoon,
            Maeday,
            Velocity
        }

        [Tooltip("A player's new stat values. Enter 0 to leave stat unchanged.")]
        [SerializeField] protected Vector4 newStats;

        [Tooltip("The player whose stats are being modified")]
        [SerializeField] protected CharacterNames characterName;

        #region Public members
        public override void OnEnter()
        {

            Flowchart datastoreFlowchart = GameObject.Find("DatastoreFlowchart").GetComponent<Flowchart>();
            Vector4 stats = datastoreFlowchart.GetVariable<Vector4Variable>(characterName.ToString() + "Stats").Value;

            if (newStats.w >= 1)
            {
                stats.w = newStats.w;
            }
            if (newStats.x >= 1)
            {
                stats.x = newStats.x;
            }
            if (newStats.y >= 1)
            {
                stats.y = newStats.y;
            }
            if (newStats.z >= 1)
            {
                stats.z = newStats.z;
            }

            datastoreFlowchart.GetVariable<Vector4Variable>(characterName.ToString() + "Stats").Apply(SetOperator.Assign, stats);

            Continue();
        }

        public override string GetSummary()
        {
            return characterName.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 204, 142, 255);
        }

        #endregion
    }
}
