using Pools;
using UnityEngine;

namespace GameplayComponents.Combat.Weapon
{
    public class PistolWeapon : BaseWeapon
    {
        [SerializeField]private MunitionPool munitionPool;
        [SerializeField]private MunitionDefinition munitionDefinition;
        [SerializeField]private Transform projectileSpawnPoint;
        [SerializeField]private float projectileSpeed = 10f;
        
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            Debug.Log("Is target null? " + (target == null));
            Debug.Log("Is target.target null? " + (target.target == null));
            var projectile = munitionPool.Get(munitionDefinition, projectileSpawnPoint.position, target.targetDirection);
            projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            onAttack.Invoke();
        }
    }
}