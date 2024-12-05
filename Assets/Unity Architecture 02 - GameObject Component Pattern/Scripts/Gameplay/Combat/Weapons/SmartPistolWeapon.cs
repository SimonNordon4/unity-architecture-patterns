using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    // Smart Pistol uses prediction on where to shoot.
    public class SmartPistolWeapon : BaseWeapon
    {
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 10f;
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if (target == null)
            {
                Debug.LogError("Target is null.");
            }
            
            var projectileStartPosition = projectileSpawnPoint.position;
            var targetPosition = target.Target.position;
            
            // Calculate the time it would take for the projectile to reach the target's current position
            var shootDirection = target.TargetDirection;
            
            if(target.Target.TryGetComponent<Movement>(out var movement))
            {
                var distanceToTarget = target.TargetDistance;
                var timeToTarget = distanceToTarget / projectileSpeed;
                var velocity = movement.velocity * 1f;
                var predictedTargetPosition = targetPosition + velocity * timeToTarget;
                
                // // now get the distance to that position
                // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
                // timeToTarget = distanceToTarget / projectileSpeed;
                // predictedTargetPosition = targetPosition + velocity * timeToTarget;
                
                // // iterate again
                // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
                // timeToTarget = distanceToTarget /projectileSpeed;
                // predictedTargetPosition = targetPosition + velocity * timeToTarget;
            
                // Aim the projectile towards the predicted position
                shootDirection = Vector3.ProjectOnPlane(predictedTargetPosition - projectileStartPosition, Vector3.up).normalized;
            }
            
            var projectile = projectilePool.Get(projectileStartPosition, shootDirection);
            
            projectile.Set(info, target.targetLayer, projectileSpeed);

            onAttack.Invoke();
        }
    }
}