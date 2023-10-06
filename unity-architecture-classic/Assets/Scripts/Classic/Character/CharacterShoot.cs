using Classic.Actor;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Character
{
    public class CharacterShooting : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private CharacterTarget target;
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform projectileSpawnPoint;

        public UnityEvent onShoot = new();

        private float _timeSinceLastShot = 0f;

        private void Update()
        {
            if(state.isGameActive == false) return;
            
            if (_timeSinceLastShot < stats.fireRate.value)
            {
                _timeSinceLastShot += Time.deltaTime;
                return;
            }

            target.GetClosestTarget();
            if (target.closestTransform is null) return;
            Debug.Log("Target acquired!");
            Shoot();
            _timeSinceLastShot = 0f;
        }

        private void Shoot()
        {
            Debug.Log("Shooting!");
            var projectileStartPosition = projectileSpawnPoint.position;
            var targetPosition = target.closestTransform.position;
            var shootDirection = (targetPosition - projectileStartPosition).normalized;
            var projectile = pool.Spawn(transform.position,shootDirection);

            projectile.Set(target.targetLayer, 10f, (int)stats.projectileDamage.value, stats.projectileKnockBack.value,
                (int)stats.projectilePierce.value);
            
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
    }
}