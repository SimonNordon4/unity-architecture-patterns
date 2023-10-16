using System;
using Classic.Actor;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Character
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private DamageReceiver damageReceiver;
        [SerializeField] private LayerMask pickupLayer;
        
        public UnityEvent onRevive = new();
        public UnityEvent onBlock = new();
        public UnityEvent onDodge = new();
        public UnityEvent<int> onHeal = new();
        public UnityEvent<int> onDamaged = new();
        public event Action onHealthChanged;

        [field:SerializeField]
        public int currentHealth { get; private set; }

        private void OnEnable()
        {
            damageReceiver.OnDamageTaken += TakeDamage;
            state.OnGameStart+=(Reset);
        }
        
        private void OnDisable()
        {
            damageReceiver.OnDamageTaken -= TakeDamage;
            state.OnGameStart-=(Reset);
        }

        public void TakeDamage(int damageAmount)
        {
            // check if blocked
            damageAmount -= (int)stats.block.value;
            if (damageAmount <= 0)
            {
                onBlock.Invoke();
                damageAmount = 0;
            }
            
            var dodgeChance = UnityEngine.Random.Range(0f, 100f);
            if (dodgeChance < stats.dodge.value)
            {
                onDodge.Invoke();
                damageAmount = 0;
            }
                
            
            currentHealth = Mathf.Clamp(
                0, 
                currentHealth - damageAmount, 
                (int)stats.playerHealth.value);
            
            onDamaged.Invoke(damageAmount);
            onHealthChanged?.Invoke();
            if (currentHealth != 0) return;
            
            if (stats.revives.value >= 1)
            {
                stats.revives.value--;
                onRevive.Invoke();
                currentHealth = (int)stats.playerHealth.value;
                return;
            }
            state.LoseGame();
        }

        public void Reset()
        {
            currentHealth = (int)stats.playerHealth.value;
            onHealthChanged?.Invoke();
        }

        // TODO: Improve.
        public void OnTriggerEnter(Collider other)
        {
            // check if other is a pickup layer
            if (pickupLayer != (pickupLayer | (1 << other.gameObject.layer))) return;

            var healthGained = (int)Mathf.Clamp( (currentHealth + stats.playerHealth.value * 0.1f + 1), 
                0f, 
                stats.playerHealth.value);
            
            currentHealth = healthGained;
            onHeal.Invoke(healthGained);
            Destroy(other.gameObject);

        }
    }
}