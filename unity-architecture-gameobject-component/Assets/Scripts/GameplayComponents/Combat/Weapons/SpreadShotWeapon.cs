using Pools;
using UnityEngine;

namespace GameplayComponents.Combat.Weapon
{
    public class SpreadShotWeapon : BaseWeapon
    {
        [SerializeField]private MunitionPool munitionPool;
        [SerializeField]private MunitionDefinition munitionDefinition;
        [SerializeField]private Transform projectileSpawnPoint;
        [SerializeField]private float projectileSpeed = 10f;
        [SerializeField]private int numberOfProjectiles = 3;
        [SerializeField]private float spreadAngle = 30f;
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            // Fire a bullet in the direction and an additional bullet offset by the spread angle
            var direction = target.targetDirection;

            // Calculate the angle between each projectile
            var angleBetweenProjectiles = numberOfProjectiles > 1 ? spreadAngle / (numberOfProjectiles - 1) : 0;

            for (var i = 0; i < numberOfProjectiles; i++)
            {
                // Calculate the offset angle
                var offsetAngle = i * angleBetweenProjectiles - spreadAngle / 2;

                // Calculate the direction of the projectile
                var projectileDirection = Quaternion.Euler(0, offsetAngle, 0) * direction;

                // Instantiate a projectile from the projectile pool
                var projectile = munitionPool.Get(munitionDefinition,projectileSpawnPoint.position, projectileDirection);
                projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            }
            
            onAttack.Invoke();
        }
    }
}