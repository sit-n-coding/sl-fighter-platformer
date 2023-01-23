using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MakersWrath.Stats
{
    public abstract class DisplayBar : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI text;

        public abstract string UpdateText();
        public abstract float UpdateValue();

        private void Update()
        {
            text.text = UpdateText();
            slider.value = UpdateValue();
        }
    }
}
