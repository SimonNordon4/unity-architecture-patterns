using System;
using Classic.Actor;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyMeleeAttack : MonoBehaviour
    {
        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private EnemyStats stats;

        private float _timeSinceLastAttack = 0f;

        private void OnEnable()
        {
            // Preload the first attack.
            _timeSinceLastAttack = stats.attackSpeed;
        }

        private void Update()
        {
            _timeSinceLastAttack += GameTime.deltaTime;
        }

        private void OnTriggerStay(Collider other)
        {
            // doing 1/attack speed inverts it. So an attack speed of 10 = 10 attacks per second.
            if(_timeSinceLastAttack < 1 / stats.attackSpeed) return;
            
            if (attackLayer == (attackLayer | (1 << other.gameObject.layer)))
            {
                Debug.Log("Attacking");
                if (!other.TryGetComponent<DamageReceiver>(out var damageReceiver)) return;
            
                Debug.Log("Dealing damage: " + stats.damage);
                Debug.Log("Damage object: " + damageReceiver.gameObject.name);
                damageReceiver.TakeDamage(stats.damage);
                _timeSinceLastAttack = 0f;
            }
        }
    }
}