using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MakersWrath.Saving;

namespace MakersWrath.Stats
{
    public class SaveManagerDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;


        public double length = 2;
        double saveTime = -1;

        private void Awake() {
            foreach (SaveManager saveManager in FindObjectsOfType<SaveManager>()) {
                // void DisplayLoad() {
                //     text.text = "LOADING...";
                //     saveTime = Time.time;
                // }
                // void DisplaySave() {
                //     text.text = "SAVING...";
                //     saveTime = Time.time;
                // }
                saveManager.onLoad.AddListener(DisplayLoad);
                saveManager.onSave.AddListener(DisplaySave);
            }
        }
        void DisplayLoad() {
                    text.text = "LOADING...";
                    saveTime = Time.time;
                }
                void DisplaySave() {
                    text.text = "SAVING...";
                    saveTime = Time.time;
                }
        private void Update()
        {
            if (length < Time.time - saveTime) {
                text.text = "";
            }
        }
    }
}
