using System.Collections;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class Projectile : MonoBehaviour
    {
        public float projectileSpeed = 10f;
        public int damage = 1;
        public float projectileLifetime = 5f;
        private float _timeAlive = 0f;
        public float knockBackIntensity = 1;
        public int pierceCount = 1;
        public float critChance = 0;
        public float critDamage = 1.5f;

        public bool canAttackPlayer = false;
        public bool canAttackEnemy = true;

        public float projectileSize = 1;

        public ParticleSystem hitEffect;

        // Update is called once per frame
        private void Update()
        {
            // set the scale of the projectile equal to the bullet size
            transform.localScale = Vector3.one * projectileSize;
            
            if (GameManager.Instance.isGameActive == false) return;

            if (_timeAlive > projectileLifetime)
                Destroy(gameObject);

            _timeAlive += Time.deltaTime;
            transform.position += transform.forward * (projectileSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (canAttackEnemy && other.CompareTag("Enemy"))
            {
                // get enemy controller component
                var enemyController = other.GetComponent<EnemyController>();

                // Check if critical hit
                var critRoll = Random.Range(0f, 100f);
                var isCritical = critChance > critRoll ;
                
                
                
                // Critical chance doubles damage.
                damage *= (isCritical ? 2 : 1);
                Debug.Log($"Is Critical? {isCritical} crit roll {critRoll} crit chance {critChance} damage: {damage}");
                enemyController.TakeDamage(damage, isCritical);

                // We have to ensure we didn't just kill the enemy.
                if (enemyController != null)
                    enemyController.ApplyKnockBack(transform.forward, knockBackIntensity);

                pierceCount--;
                if (pierceCount < 0)
                    Die();
            }

            if (canAttackPlayer && other.CompareTag("Player"))
            {
                var playerController = other.GetComponent<PlayerController>();
                playerController.TakeDamage(damage);
                pierceCount--;
                if (pierceCount <= 0)
                    Die();

            }
        }

        private void Die()
        {
            if (hitEffect != null)
            {
                var effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                GameManager.Instance.StartCoroutine(DestroyAfter(effect));
            }
            Destroy(gameObject);
        }

        private IEnumerator DestroyAfter(ParticleSystem effect)
        {
            yield return new WaitForSeconds(1f);
            if(effect!=null)
                Destroy(effect.gameObject);
        }
    }
}