using Classic.Character;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actors
{
    public class ActorRangedWeapon : MonoBehaviour
    {
        [SerializeField] private ActorStats stats;
        [SerializeField] private ActorTarget target;
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 10f;

        public UnityEvent onShoot = new();

        private float _timeSinceLastShot = 0f;

        private void Update()
        {
            if (_timeSinceLastShot < stats.Map[StatType.RangedAttackSpeed].value)
            {
                _timeSinceLastShot += Time.deltaTime;
                return;
            }

            Shoot();
            _timeSinceLastShot = 0f;
        }

        private void Shoot()
        {
            var range = stats.Map[StatType.RangedRange].value;
            target.GetClosestTarget(range);
            if (target.hasTarget == false) return;
            
            var projectile = pool.Spawn(projectileSpawnPoint.position, target.targetDirection);
            
            var layer = target.targetLayer;
            var damage = (int)stats.Map[StatType.RangedDamage].value;
            var knockBack = stats.Map[StatType.RangedKnockBack].value;
            var pierce = (int)stats.Map[StatType.RangedPierce].value;

            projectile.Set(layer, projectileSpeed, damage, knockBack, pierce);
            
            onShoot.Invoke();
        }

        private void ShootPredictive()
        {
            // var projectileStartPosition = projectileSpawnPoint.position;
            // var targetPosition = target.closestTransform.position;
            // var projectile = Instantiate(projectilePrefab, projectileStartPosition, Quaternion.identity);
            //
            //
            // // Calculate the time it would take for the projectile to reach the target's current position
            //
            // var distanceToTarget = Vector3.Distance(projectileStartPosition, targetPosition);
            // var timeToTarget = distanceToTarget / projectile.projectileSpeed;
            //
            // // Predict the target's position after the time it would take for the projectile to reach it
            // var enemy = target.closestTransform.GetComponent<EnemyController>();
            // var enemyVelocity = target.closestTransform.forward * enemy.moveSpeed;
            // var predictedTargetPosition = targetPosition + enemyVelocity * timeToTarget;
            //
            // // now get the distance to that position
            // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
            // timeToTarget = distanceToTarget / projectile.projectileSpeed;
            // predictedTargetPosition = targetPosition + enemyVelocity * timeToTarget;
            //
            // // iterate again
            // distanceToTarget = Vector3.Distance(targetPosition, predictedTargetPosition);
            // timeToTarget = distanceToTarget / projectile.projectileSpeed;
            // predictedTargetPosition = targetPosition + enemyVelocity * timeToTarget;
            //
            // // Aim the projectile towards the predicted position
            // var shootDirection = (predictedTargetPosition - projectileStartPosition).normalized;
            //
            //
            // projectile.transform.forward = shootDirection;
            // projectile.damage = Mathf.RoundToInt(GameManager.instance.pistolDamage.value);
            // projectile.knockBackIntensity = GameManager.instance.pistolKnockBack.value;
            // projectile.pierceCount = (int)GameManager.instance.pistolPierce.value;
            
            onShoot.Invoke();
        }

        public void Reset()
        {
            _timeSinceLastShot = 0.0f;
        }
    }
}