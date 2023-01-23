using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace MakersWrath.Saving
{
    /// <summary>
    /// Class for entities to compose that have the ability to save and load.
    /// </summary>

    // NOTE: Should we have save & load errors if they don't happen?
    public class SaveManager : MonoBehaviour {

        // state vars
        private Dictionary<string, object> saveState = new Dictionary<string, object>();
        public bool hasSaveState = false;

        [SerializeField] public UnityEvent onLoad;
        [SerializeField] public UnityEvent onSave;

        public virtual void Save()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            saveState = state;
            onSave.Invoke();
            hasSaveState = true;
        }

        public virtual void Load()
        {
            if (!hasSaveState) 
            {
                return;
            }
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (saveState.ContainsKey(id))
                {
                    saveable.RestoreState(saveState[id]);
                }
            }
            onLoad.Invoke();
            hasSaveState = false;
        }

    }
}