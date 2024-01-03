using GameplayComponents.Combat;
using GameplayComponents.Life;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class BodySlamWeapon : BaseWeapon
    {
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            Debug.Log("Body Slamming!");
            if(!target.target.TryGetComponent<DamageReceiver>(out var receiver))
                return;
            Debug.Log("Dealing damage!");
            receiver.TakeDamage(info.Damage);
        }
    }
}