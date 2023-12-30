using System;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents.Life
{
    public class Health : GameplayComponent
    {
        [field:SerializeField] public int currentHealth { get; private set; }
        [SerializeField] private Stats stats;
        private Stat _maxHealthStat;
        public int maxHealth => (int)_maxHealthStat.value;
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;

        private void Start()
        {
            _maxHealthStat = stats.GetStat(StatType.MaxHealth);
            SetHealth(maxHealth);
        }

        public void SetHealth(int health)
        {
            currentHealth = health;
            OnHealthChanged?.Invoke(currentHealth);
        }
        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0) return;

            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth > 0) return;
            OnDeath?.Invoke();
        }

        private void OnDestroy()
        {
            OnHealthChanged = null;
            OnDeath = null; 
        }
    }
}