using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    
    [Header("References")]
    public EnemyManager enemyManager;
    
    [Header("Movement")]
    public Transform playerTarget;
    public float moveSpeed = 5f;
    public float repulsionForce = 0.5f;

    [Header("Health")] 
    public int currentHealth = 5;
    
    [Header("Attack")]
    public int damageAmount = 1;
    public float damageCooldown = 0.2f;
    private float _timeSinceLastDamage;
    
    private readonly List<Transform> _nearbyEnemies = new(4);

    private void Update()
    {
        if(GameManager.instance.isGameActive == false) return;
        
        // Direction towards player.
        if (playerTarget == null) return;
        var dir =  Vector3.ProjectOnPlane(playerTarget.position - transform.position,Vector3.up).normalized;
        
        // Push away from close enemies.
        // TODO: We need to make the avoidance direction STRONGER the closer the enemy is.
        // The avoidance direction will be zero at 1m apart.
        // We will add the avoidance directions together, and then divide by total enemies to scale it down.
        // This means if the enemies are overlapping, avoidance will contribute 50% of the direction.
        // This will drop off to zero based on distance.
        var totalEnemies = _nearbyEnemies.Count;
        var avoidanceDirection = Vector3.zero;
        for(var i = 0; i < totalEnemies; i++)
        {
            var enemy = _nearbyEnemies[i];
            if (enemy == null)
            {
                _nearbyEnemies.RemoveAt(i);
                i--;  // Adjust the index to account for the removed enemy
                totalEnemies--;  // Adjust the total count of enemies
                continue;
            }
            var toEnemy = enemy.position - transform.position;

            // Calculate distance and project direction on plane
            var distance = toEnemy.magnitude;
            var enemyDir = Vector3.ProjectOnPlane(toEnemy, Vector3.up).normalized;

            // Scale the direction based on distance (closer enemies have stronger influence)
            if (distance < 1.0f)  // We don't want to divide by zero or a negative number
            {
                // Using the inverse of distance to scale direction
                var scale = 1.0f - distance;
                avoidanceDirection -= enemyDir * scale;
            }
        }
        
        
        var updatedDir = (dir + avoidanceDirection * repulsionForce).normalized;

        // Apply direction to transform.
        if (dir.magnitude > 0)
        {
            transform.position += updatedDir * (Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.LookRotation(dir);
        }
        
        // Track Attack cooldown.
        _timeSinceLastDamage += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        // Take damage, die if at 0.
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            enemyManager.EnemyDied(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add nearby enemies to list.
        if (other.CompareTag("Enemy"))
        {
            if(_nearbyEnemies.Count < 4)
                _nearbyEnemies.Add(other.transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously damage the player.
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (_timeSinceLastDamage > damageCooldown)
                {
                    playerController.TakeDamage(damageAmount);
                    _timeSinceLastDamage = 0f;
                }

            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Remove nearby enemies from list.
        if (other.CompareTag("Enemy"))
        {
            if(_nearbyEnemies.Contains(other.transform))
                _nearbyEnemies.Remove(other.transform);
        }
    }
}
