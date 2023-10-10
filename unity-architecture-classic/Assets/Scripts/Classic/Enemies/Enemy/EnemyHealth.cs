using Classic.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Enemies.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        [SerializeField] private EnemyStats stats;
        [SerializeField] private DamageReceiver damageReceiver;

        private int _currentHealth;

        private void OnEnable()
        {
            damageReceiver.OnDamageTaken += DamageTaken;
            _currentHealth = stats.health;
        }

        private void DamageTaken(int amount)
        {
            scope.enemyEvents.onEnemyDamaged(amount, transform.position);
            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, stats.health);

            if (_currentHealth > 0) return;
            scope.enemyEvents.onEnemyDied(scope, transform.position);
            scope.Destroy();
        }
    }
}