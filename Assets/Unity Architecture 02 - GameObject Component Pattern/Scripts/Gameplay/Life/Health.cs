using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Stats))]
    public class Health : MonoBehaviour
    {
        [field: SerializeField] public int currentHealth { get; private set; }
        private Stat _maxHealthStat;
        public int maxHealth => _maxHealthStat.value;

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnHealthDepleted;

        private void Awake()
        {
            _maxHealthStat = GetComponent<Stats>().GetStat(StatType.MaxHealth);
        }

        private void OnEnable()
        {
             _maxHealthStat.onModifierAdded += OnMaxHealthModifierAdded;
        }

        private void OnDisable()
        {
            _maxHealthStat.onModifierAdded -= OnMaxHealthModifierAdded;
        }

        private void OnMaxHealthModifierAdded(Modifier hpMod)
        {
            var hpChange = hpMod.modifierValue;

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
    }
}