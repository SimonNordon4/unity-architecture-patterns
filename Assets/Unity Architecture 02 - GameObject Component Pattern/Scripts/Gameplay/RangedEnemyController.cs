using System.Collections;
using System.Collections.Generic;
using UnityArchitecture.GameObjectComponentPattern;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class RangedEnemyController : EnemyController
    {
        [Header("Ranged Attributes")] public GameObject projectilePrefab;
        public float range = 6f;
        public float targetDistance = 5.5f;
        public int bulletCount = 1;
        public float projectileSpeed = 2f;
        public bool startLoaded = true;
        public float bulletSpread = 15f;
        public float inaccuracySpread = 15f;

        protected override void Start()
        {
            base.Start();
            // start with a bullet in the chamber!
            if (startLoaded)
                _timeSinceLastDamage = damageCooldown;
        }

        protected override void TowardsPlayer()
        {

        }

        protected override void Update()
        {
            if (GameManager.Instance.isGameActive == false) return;

            // If the enemy is spawning, then do nothing other than play the spawn animation and wait.
            if (isSpawning)
            {
                _spawnTimeElapsed += Time.deltaTime;
                if (_spawnTimeElapsed >= spawnTime)
                {
                    isSpawning = false;
                    deathEffect.Play();
                    spawnEffect.Stop();
                    meshObject.SetActive(true);
                    shadowObject.SetActive(true);
                    if (UserSettings.Instance.ShowEnemyHealthBars)
                    {
                        healthBarUI.SetActive(true);
                    }
                }
                
                return;
            }
            
            if (isKnockedBack) return;


            if (playerTarget == null) return;
            var difference = Vector3.ProjectOnPlane(playerTarget.position - transform.position, Vector3.up);
            var distance = difference.magnitude;
            difference = difference.normalized;

            if (moveBehaviour == MoveBehaviour.TowardsPlayer)
            {
                var wishDir = Vector3.zero;

                // Move towards if far away, move away if too close, don't move if just right.
                if (distance < targetDistance - 0.5f)
                    wishDir = -difference.normalized;

                else if (distance > targetDistance + 0.5f)
                    wishDir = difference.normalized;

                var avoidanceDirection = GetAvoidanceFromOtherEnemies();
                var updatedDir = (wishDir + avoidanceDirection * repulsionForce).normalized;

                transform.position += updatedDir * (Time.deltaTime * moveSpeed);
                if (distance > 0) transform.rotation = Quaternion.LookRotation(difference);
            }
            else if (moveBehaviour == MoveBehaviour.RandomLocation)
            {
                var position = transform.position;
                var projectedPosition = new Vector3(position.x, 0, position.z);
                var dir = Vector3.ProjectOnPlane(randomPosition - projectedPosition, Vector3.up).normalized;
                var distanceToRandom = Vector3.Distance(randomPosition, projectedPosition);

                // tolerance is the radius of the enemy.
                var tolerance = transform.localScale.x * 0.5f;

                if (distanceToRandom < tolerance + 0.5f)
                {
                    randomPosition = new Vector3(Random.Range(GameManager.Instance.levelBounds.x * -1, GameManager.Instance.levelBounds.x), 0, Random.Range(GameManager.Instance.levelBounds.y * -1, GameManager.Instance.levelBounds.y));
                }

                var avoidanceDirection = GetAvoidanceFromOtherEnemies();
                var updatedDir = (dir + avoidanceDirection * repulsionForce).normalized;

                // Apply direction to transform.
                if (dir.magnitude > 0)
                {
                    transform.position += updatedDir * (Time.deltaTime * moveSpeed);
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }

            ClampTransformToLevelBounds();

            _timeSinceLastDamage += Time.deltaTime;
            if (_timeSinceLastDamage > damageCooldown && difference.magnitude < range)
            {
                Shoot(difference);
                _timeSinceLastDamage = 0;
            }

            healthBarUI.transform.rotation = uiStartRotation;
        }


        private void Shoot(Vector3 direction)
        {
            // Calculate total spread angle.
            var totalSpread = bulletSpread * (bulletCount - 1);

            // Determine the starting angle. If the bulletCount is odd, 
            // there will always be one bullet going directly forward.
            var startAngle = bulletCount % 2 == 1 ? -(totalSpread / 2) : -totalSpread / 2 + bulletSpread / 2;

            // add a 15 degree random spread to the bullet.
            startAngle += Random.Range(-inaccuracySpread, inaccuracySpread);

            // Get the rotation that will take the Vector3.forward to the 'direction' vector
            var toDirection = Quaternion.FromToRotation(Vector3.forward, direction);

            for (var i = 0; i < bulletCount; i++)
            {
                // Rotate the direction by the current angle to get the bullet direction
                var dir = toDirection * Quaternion.Euler(0, startAngle + i * bulletSpread, 0) * Vector3.forward;

                // Create and setup the projectile
                var projectileGo = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(dir));
                var projectile = projectileGo.GetComponent<Old_Projectile>();
                projectile.damage = currentDamage;
                projectile.projectileSpeed = projectileSpeed;
            }

            _timeSinceLastDamage = 0;
        }
    }
}