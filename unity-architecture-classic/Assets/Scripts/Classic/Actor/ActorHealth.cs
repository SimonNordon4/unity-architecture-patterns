using System;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actor
{
    public class ActorHealth : ActorComponent
    {
        [SerializeField] private int currentHealth;
        
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;
        
        public void SetHealth(int health)
        {
            currentHealth = health;
        }
        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0) return;
            
            currentHealth -= damageAmount;
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth > 0) return;
            
            currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}