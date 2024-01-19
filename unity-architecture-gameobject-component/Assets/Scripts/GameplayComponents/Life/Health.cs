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
        
        public Action<int> OnHealthChanged;
        public Action OnHealthDepleted;

        private void Awake()
        {
            _maxHealthStat = stats.GetStat(StatType.MaxHealth);
            _maxHealthStat.onModifierAdded += OnMaxHealthModifierAdded;
        }

        private void OnDestroy()
        {
            _maxHealthStat.onModifierAdded -= OnMaxHealthModifierAdded;
        }

        private void OnMaxHealthModifierAdded(Modifier hpMod)
        {
            // Calculate the difference between the new max health and the old max health
            if (hpMod.modifierType == ModifierType.Flat)
            {
                var hpChange = (int)hpMod.modifierValue;

                // If the max health has increased, increase the current health by the difference
                if (hpChange > 0)
                {
                    currentHealth += hpChange;
                }
                // If the max health has decreased, clamp the current health to the new max health if it's greater than the new max health
                else if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
            }

            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Start()
        {
            SetHealth(maxHealth);
        }

        public void SetHealth(int health)
        {
            currentHealth = health;
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        public void AddHealth(int health)
        {
            if (health <= 0) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + health);
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0) return;

            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            OnHealthChanged?.Invoke(currentHealth);
            
            if (currentHealth > 0) return;
            
            OnHealthDepleted?.Invoke();
        }

        public void SetToMaxHealth()
        {
            currentHealth = maxHealth;
        }
        
        public override void OnGameStart()
        {
            _maxHealthStat = stats.GetStat(StatType.MaxHealth);
            SetHealth(maxHealth);
        }
    }
}