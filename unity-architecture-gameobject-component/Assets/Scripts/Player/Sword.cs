using GameplayComponents.Actor;
using GameplayComponents.Life;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameplayComponents.Combat.Weapon
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        private CombatTarget _target;
        
        private int _damage;
        private float _knockBack;
        
        public void Set(WeaponStatsInfo info, CombatTarget target)
        {
            _target = target;
            _damage = info.Damage;
            _knockBack = info.KnockBack;
        }

        private void OnTriggerEnter(Collider other)
        {
            // check if other is on character target layer
            if (_target.targetLayer != (_target.targetLayer | (1 << other.gameObject.layer))) return;
            
            if(other.TryGetComponent<DamageReceiver>(out var damageReceiver))
                damageReceiver.TakeDamage(_damage);

            if (other.TryGetComponent<KnockBackReceiver>(out var knockBackReceiver))
            {
                knockBackReceiver.ApplyKnockBack(_target.targetDirection * _knockBack);
            }
        }
    }
}