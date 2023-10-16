using System;
using Classic.Actors;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyMeleeAttack : MonoBehaviour
    {
        [SerializeField] private EnemyStats stats;

        private float _timeSinceLastAttack = 0f;

        private void Update()
        {
            _timeSinceLastAttack += GameTime.deltaTime;
        }

        private void OnTriggerStay(Collider other)
        {
            // doing 1/attack speed inverts it. So an attack speed of 10 = 10 attacks per second.
            if(_timeSinceLastAttack < 1 / stats.attackSpeed) return;

            if (stats.attackLayer != (stats.attackLayer | (1 << other.gameObject.layer))) return;
            
            if (!other.TryGetComponent<DamageReceiver>(out var damageReceiver)) return;

            damageReceiver.TakeDamage(stats.damage);
            _timeSinceLastAttack = 0f;
        }
    }
}