using GameObjectComponent.GameplayComponents.Life;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Combat.Weapons
{
    [RequireComponent(typeof(CombatTarget))]
    public class BodySlamWeapon : BaseWeapon
    {

        private CombatTarget _targetComponent;

        private void Start()
        {
            _targetComponent = GetComponent<CombatTarget>();
        }

        public override void Attack(MeleeStatsInfo info)
        {
            if(!_targetComponent.target.TryGetComponent<DamageReceiver>(out var receiver)) return;
            receiver.TakeDamage(info.Damage);
        }
    }
}