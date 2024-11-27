using GameplayComponents.Combat;
using GameplayComponents.Life;
using UnityEngine;

namespace GameplayComponents.Combat.Weapons
{
    public class BodySlamWeapon : BaseWeapon
    {
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if(!target.target.TryGetComponent<DamageReceiver>(out var receiver))
                return;
            receiver.TakeDamage(info.Damage);
            onAttack?.Invoke();
        }
    }
}