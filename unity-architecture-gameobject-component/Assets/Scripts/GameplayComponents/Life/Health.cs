using System;
using GameObjectComponent.GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Life
{
    public class Health : GameplayComponent
    {
        [field:SerializeField] public int currentHealth { get; private set; }
        [SerializeField] private ActorStats stats;
        public int maxHealth => (int)stats.Map[StatType.MaxHealth].value;
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;

        private void OnEnable()
        {
            Reset();
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