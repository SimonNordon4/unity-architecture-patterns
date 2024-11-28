using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class BodySlamWeapon : BaseWeapon
    {
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if(!target.Target.TryGetComponent<DamageReceiver>(out var receiver))
                return;
            receiver.TakeDamage(info.Damage);
            onAttack?.Invoke();
        }
    }
}