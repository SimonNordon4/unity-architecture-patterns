using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
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

// Calculate predicted position
            Vector3 directionToTarget = targetPosition - projectileStartPosition;
            float distanceToTarget = directionToTarget.magnitude;
            float timeToTarget = distanceToTarget / projectileSpeed;

            Vector3 predictedTargetPosition = targetPosition;

            if (target.Target.TryGetComponent<Movement>(out var movement))
            {
                var predictedPosition = targetPosition + movement.velocity * timeToTarget;
                // If predicted position ends up behind you, revert to direct line-of-sight attack
                var forwardDirection = (projectileSpawnPoint.forward).normalized;
                var predictedDirection = (predictedPosition - projectileStartPosition).normalized;

                if (Vector3.Dot(forwardDirection, predictedDirection) < 0f)
                {
                    // Predicted position is behind, fallback to direct direction
                    predictedTargetPosition = targetPosition;
                }
                else
                {
                    predictedTargetPosition = predictedPosition;
                }
            }

// Now calculate the shoot direction on the plane
            var shootDirection = Vector3.ProjectOnPlane(predictedTargetPosition - projectileStartPosition, Vector3.up)
                .normalized;
            var projectile = projectilePool.Get(projectileStartPosition, shootDirection);
            projectile.Set(info, target.targetLayer, projectileSpeed);
            onAttack.Invoke();
        }
    }
}