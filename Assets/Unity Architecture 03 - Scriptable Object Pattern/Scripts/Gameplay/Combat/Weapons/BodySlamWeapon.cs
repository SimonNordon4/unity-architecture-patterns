using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class BodySlamWeapon : BaseWeapon
    {
        public override void Attack(WeaponStatsInfo info, CombatTarget target, Transform origin)
        {
            if(!target.Target.TryGetComponent<DamageReceiver>(out var receiver))
                return;
            receiver.TakeDamage(info.Damage);
            onAttack?.Invoke();
        }
    }
}