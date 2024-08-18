using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// EVN-specific LoadScene command for transferring DatastoreFlowchart variables to the next scene.
    /// </summary>
    [CommandInfo("EVN",
                 "Load Day",
                 "For going from one VN scene to another. Uses a DatastoreLink to carry this day's DatastoreFlowchart variables to the next.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LoadDay : LoadScene
    {
        #region Public members

        public override void OnEnter()
        {
            DatastoreLink datastoreLink = GameObject.Find("DatastoreLink").GetComponent<DatastoreLink>();
            if (datastoreLink != null)
            {
                datastoreLink.LoadNextScene();
            }
            else
            {
                Debug.LogError("Called 'Load Day', but no DatastoreLink in this scene. Add a DatastoreLink prefab to this scene, or use the base 'Load Scene' command.");
            }

            base.OnEnter();
        }

        public override string GetSummary()
        {
            return base.GetSummary();
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 204, 142, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        #endregion
    }
}
