using UnityEngine;
using UnityEngine.Events;
using MakersWrath.Saving;

namespace MakersWrath.Stats
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float initialHealthPoints;
        [SerializeField] UnityEvent onDeath;
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { };
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] float healthPoints;
        
        bool isDead = false;

        #region Getters/Setters

        public bool IsDead { get { return isDead; } }
        
        public float HealthPoints { get { return healthPoints; } }

        public float InitialHealthPoints { get { return initialHealthPoints; } }

        #endregion

        public void TakeDamage(float damage)
        {
            if (healthPoints <= 0)
            {
                Debug.LogWarning(gameObject.name + " has no hp");
                return;
            }

            healthPoints = Mathf.Max(healthPoints - damage, 0);
            takeDamage.Invoke(damage);

            if (healthPoints == 0)
            {
                onDeath.Invoke();
                takeDamage.Invoke(damage);
                TriggerDeath();
            }
        }

        private void TriggerDeath()
        {
            if (isDead) return;

            isDead = true;
            Destroy(gameObject);
        }

        private void Awake()
        {
            healthPoints = initialHealthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
        }

        public object CaptureState()
        {
            return healthPoints;
        }
    }
}
