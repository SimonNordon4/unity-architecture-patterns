using UnityEngine.Events;

namespace GameplayComponents.Combat.Weapons
{
    public abstract class BaseWeapon : GameplayComponent
    {
        public abstract void Attack(WeaponStatsInfo info, CombatTarget target);

        public UnityEvent onAttack = new();
    }
}