using Pools;
using UnityEngine;

namespace GameplayComponents.Combat.Weapon
{
    public class ExplodeProjectileWeapon : BaseWeapon
    {
        [SerializeField]private ProjectileDefinition projectileDefinition;
        [SerializeField]private Transform projectileSpawnPoint;
        [SerializeField]private int numberOfProjectiles = 3;
        [SerializeField] private MunitionPool pool;
        [SerializeField] private float projectileSpeed = 5f;
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            // find the average angle of the cirlce to evenly shoot all projectiles.
            var angle = 360f / numberOfProjectiles;
            
            // loop through the number of projectiles and spawn them
            for (var i = 0; i < numberOfProjectiles; i++)
            {
                // calculate the angle of the projectile
                var projectileAngle = angle * i;
                
                // calculate the direction of the projectile
                var direction = Quaternion.Euler(0, projectileAngle, 0) * transform.forward;
                
                // spawn the projectile
                var projectile = pool.Get(projectileDefinition, projectileSpawnPoint.position, direction);
                
                // set the projectile stats
                projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            }
            
            onAttack.Invoke();
        }
    }
}