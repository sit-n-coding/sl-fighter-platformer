using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace MakersWrath.Saving
{
    /// <summary>
    /// Class for entities to compose that have the ability to save and load.
    /// </summary>

    // NOTE: Should we have save & load errors if they don't happen?
    public class TimedSaveManager : SaveManager {

        // state vars
        private Dictionary<string, object> saveState = new Dictionary<string, object>();

        // vars for timers
        public double saveExpiry = -1; // set in UI, in seconds if != -1, o/w DNE
        public double loadCooldown = -1;  // set in UI, in seconds if != -1, o/w DNE
        public double saveTime = 0;
        public double loadTime = 0;

        public override void Save()
        {
            if (((loadTime == 0) && (loadCooldown != -1)) || (loadCooldown > Time.time - loadTime)) 
            {
                return;
            }
            base.Save();
            saveTime = Time.time;
        }

        public override void Load()
        {
            if (!hasSaveState || ((saveExpiry != -1) && (saveExpiry < Time.time - saveTime))) 
            {
                return;
            }
            base.Load();
            loadTime = Time.time;
        }

    }
}