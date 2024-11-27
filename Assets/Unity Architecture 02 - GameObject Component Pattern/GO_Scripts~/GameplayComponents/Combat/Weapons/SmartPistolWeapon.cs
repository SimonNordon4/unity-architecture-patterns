using GameplayComponents.Locomotion;
using Pools;
using UnityEngine;

namespace GameplayComponents.Combat.Weapons
{
    // Smart Pistol uses prediction on where to shoot.
    public class SmartPistolWeapon : BaseWeapon
    {
        [SerializeField] private MunitionPool munitionPool;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private MunitionDefinition munitionDefinition;
        [SerializeField] private float projectileSpeed = 10f;
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if (target == null)
            {
                Debug.LogError("Target is null.");
            }
            
            var projectileStartPosition = projectileSpawnPoint.position;
            var targetPosition = target.target.position;
            
            // Calculate the time it would take for the projectile to reach the target's current position
            var shootDirection = target.targetDirection;
            
            if(target.target.TryGetComponent<Movement>(out var movement))
            {
                var distanceToTarget = target.targetDistance;
                var timeToTarget = distanceToTarget / projectileSpeed;
                var velocity = movement.velocity;
                var predictedTargetPosition = targetPosition + velocity * timeToTarget;
                
                // // now get the distance to that position
                // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
                // timeToTarget = distanceToTarget / projectileSpeed;
                // predictedTargetPosition = targetPosition + velocity * timeToTarget;
                //
                // // iterate again
                // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
                // timeToTarget = distanceToTarget /projectileSpeed;
                // predictedTargetPosition = targetPosition + velocity * timeToTarget;
            
                // Aim the projectile towards the predicted position
                shootDirection = Vector3.ProjectOnPlane(predictedTargetPosition - projectileStartPosition, Vector3.up).normalized;
            }
            
            var projectile = munitionPool.Get(munitionDefinition, projectileStartPosition, shootDirection);
            
            projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);

            onAttack.Invoke();
        }
    }
}