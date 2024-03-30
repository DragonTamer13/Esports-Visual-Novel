using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Autosave",
                 "Create a save file and screenshot in persistent memory. The oldest autosave is deleted if there are too many saves.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Autosave : Command
    {
        // Constants
        public const int MaxAutosaves = 4;
        public static readonly string AutosavePrefix = "auto";

        private static string GetSaveImageName(string saveDataKey)
        {
            return Application.persistentDataPath + "/" + saveDataKey + ".png";
        }

        #region Public members
        public override void OnEnter()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            string lastAutosaveName = AutosavePrefix + (MaxAutosaves - 1).ToString();

            if (saveManager.SaveDataExists(lastAutosaveName))
            {
                SaveManager.Delete(lastAutosaveName);
                SaveManager.DeleteScreenshot(lastAutosaveName);
            }

            // Shift the saves down
            string toName = "";
            string fromName = lastAutosaveName;
            for (int i = MaxAutosaves - 2; i > -1; i--)
            {
                toName = fromName;
                fromName = AutosavePrefix + i.ToString();
                if (saveManager.SaveDataExists(fromName))
                {
                    // NOTE: There might be a race condition where a save file is overwritten before it is done being copied to the next save slot.
                    //       Didn't happen while testing.
                    System.IO.File.Move(SaveManager.GetFullFilePath(fromName), SaveManager.GetFullFilePath(toName));
                    System.IO.File.Move(GetSaveImageName(fromName), GetSaveImageName(toName));
                }
            }

            // Create the new autosave
            saveManager.Save(AutosavePrefix + "0");
            StartCoroutine(saveManager.TakeSaveScreenshot(AutosavePrefix + "0"));

            print("Autosave worked");

            Continue();


            /// <summary>
            /// Remake the save system code flow. The idea is that the new save system is designed with the intent that you want to take a screenshot
            /// when you make a save.
            /// 
            /// 1. Create a new Command subclass for autosaving. Calls a function on FungusManager.Instance.SaveManager. This way autosaving is removed
            ///    from the SaveLoadMenu.
            /// 2. Add a function to SaveManager class for saving and taking a screenshot (ie. saving the way we want to for this project):
            ///    SaveAndScreenshot(string saveDataKey, CanvasGroup canvasGroup, System.Action onCompleteAction)
            /// 3. Does the same thing as the Save() function here, but within SaveManager. Starts the coroutine to take a screenshot and calls the
            ///    Action when done. canvasGroup visibility is disabled while taking the screenshot.
            /// </summary>
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
