using Classic.Actors;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Enemies.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private DamageReceiver damageReceiver;
        [SerializeField] private EnemyScope scope;
        [SerializeField] private EnemyStats stats;
        public int currentHealth;

        private void OnEnable()
        {
            currentHealth = stats.baseHealth;
            damageReceiver.OnDamageReceived += TakeDamage;
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived -= TakeDamage;
        }

        private void TakeDamage(int damage)
        {
            currentHealth -= damage;
            scope.events.OnEnemyDamaged?.Invoke(transform.position, damage);

            if (currentHealth > 0) return;
            currentHealth = 0;
            scope.events.OnEnemyDeath?.Invoke(transform.position);
            Destroy(gameObject);
        }
    }
}