using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MakersWrath.Saving;
using System;

namespace MakersWrath.Stats
{
    public class LoadDisplay : DisplayBar
    {

        TimedSaveManager saveManager;

        private void Awake()
        {
            saveManager = GameObject.FindWithTag("Player").GetComponent<TimedSaveManager>();
            if (saveManager == null)
            {
                Debug.LogError("Unable to get player health component");
            }
        }

        public float LoadReady() {
            if (!saveManager.hasSaveState) return 0;
            if (saveManager.saveExpiry == -1) return 1;
            return 1 - Mathf.Min((float) ((Time.time - saveManager.saveTime) / saveManager.saveExpiry), 1);
        }

        public override string UpdateText() {
            return Mathf.Ceil(Mathf.Ceil(LoadReady() * 100)).ToString();
        }

        public override float UpdateValue() {
            return LoadReady();
        }
    }
}
