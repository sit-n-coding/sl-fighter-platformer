using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MakersWrath.Stats
{
    public class HealthDisplay : DisplayBar
    {
        public GameObject userWithHealth;
        Health health;

        private void Awake()
        {
            health = userWithHealth.GetComponent<Health>();
            if (health == null)
            {
                Debug.LogError("Unable to get player health component");
            }
        }

        public override string UpdateText() {
            return Mathf.Ceil(health.HealthPoints).ToString();
        }

        public override float UpdateValue() {
            return health.HealthPoints / health.InitialHealthPoints;
        }
    }
}
