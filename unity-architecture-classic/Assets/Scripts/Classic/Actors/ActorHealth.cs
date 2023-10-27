using System;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actors
{
    public class ActorHealth : ActorComponent
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private ActorStats stats;
        private Stat _maxHealthStat;
        
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;

        private void Start()
        {
            Reset();
        }

        public void SetHealth(int health)
        {
            currentHealth = health;
        }
        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0) return;

            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth > 0) return;
            OnDeath?.Invoke();
        }

        public override void Reset()
        {
            currentHealth = (int)stats.Map[StatType.MaxHealth].value;
        }

        private void OnDestroy()
        {
            OnHealthChanged = null;
            OnDeath = null; 
        }
    }
}