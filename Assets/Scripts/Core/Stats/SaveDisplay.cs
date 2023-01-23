using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MakersWrath.Saving;
using System;

namespace MakersWrath.Stats
{
    public class SaveDisplay : DisplayBar
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

        public float SaveReady() {
            if (saveManager.loadCooldown == -1) return 1;
            return Mathf.Min((float) ((Time.time - saveManager.loadTime) / saveManager.loadCooldown), 1);
        }

        public override string UpdateText() {
            return Mathf.Ceil(Mathf.Ceil(SaveReady() * 100)).ToString();
        }

        public override float UpdateValue() {
            return SaveReady();
        }
    }
}
