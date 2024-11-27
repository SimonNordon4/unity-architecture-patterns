using Pools;
using UnityEngine;

namespace GameplayComponents.Combat.Weapons
{
    public class PistolWeapon : BaseWeapon
    {
        [SerializeField]private MunitionPool munitionPool;
        [SerializeField]private MunitionDefinition munitionDefinition;
        [SerializeField]private Transform projectileSpawnPoint;
        [SerializeField]private float projectileSpeed = 10f;
        
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {

            var projectile = munitionPool.Get(munitionDefinition, projectileSpawnPoint.position, target.targetDirection);
            projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            onAttack.Invoke();
        }
    }
}