using Classic.Actor;
using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private DamageReceiver damageReceiver;

        [field:SerializeField]
        public int currentHealth { get; private set; }

        private void OnEnable()
        {
            damageReceiver.OnDamageTaken += TakeDamage;
            state.onGameStart.AddListener(Reset);
        }
        
        private void OnDisable()
        {
            damageReceiver.OnDamageTaken -= TakeDamage;
            state.onGameStart.RemoveListener(Reset);
        }

        private void TakeDamage(int damageAmount)
        {
            currentHealth = Mathf.Clamp(
                0, 
                currentHealth - damageAmount, 
                (int)stats.playerHealth.value);
            
            if (currentHealth == 0)
            {
                state.LoseGame();
            }
        }

        private void Reset()
        {
            currentHealth = (int)stats.playerHealth.value;
        }
        
        
    }
}