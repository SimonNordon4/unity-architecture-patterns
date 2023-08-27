using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public EnemyManager enemyManager;
    
    [Header("UI")]
    public GameObject healthBarUI;
    public TextMeshProUGUI healthText;
    protected Quaternion uiStartRotation;
    
    [Header("Movement")]
    public Transform playerTarget;
    public float moveSpeed = 5f;
    public float repulsionForce = 0.5f;
    public float knockBackFactor = 1f;
    protected bool _isKnockedBack = false;

    [Header("Health")] 
    public int currentHealth = 5;
    
    [Header("Attack")]
    public int damageAmount = 1;
    public float damageCooldown = 0.2f;
    protected float _timeSinceLastDamage;
    protected readonly List<Transform> _nearbyEnemies = new(4);

    protected virtual void Start()
    {
        // de-parent so we don't follow the enemy rotation.
        uiStartRotation = healthBarUI.transform.rotation;
        healthBarUI.SetActive(SettingsManager.instance.showEnemyHealthBars);
        UpdateHealthText();
    }

    protected virtual void Update()
    {
        if(GameManager.instance.isGameActive == false) return;
        
        if(_isKnockedBack) return;
        
        if (playerTarget == null) return;
        var dir =  Vector3.ProjectOnPlane(playerTarget.position - transform.position,Vector3.up).normalized;
        if (dir.magnitude < 0.5f)
        {
            dir = Vector3.zero;
        }

        var avoidanceDirection = GetAvoidanceFromOtherEnemies();
        var updatedDir = (dir + avoidanceDirection * repulsionForce).normalized;

        // Apply direction to transform.
        if (dir.magnitude > 0)
        {
            transform.position += updatedDir * (Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.LookRotation(dir);
        }
        
        ClampTransformToLevelBounds();
        
        // Track Attack cooldown.
        _timeSinceLastDamage += Time.deltaTime;
        
        healthBarUI.transform.rotation = uiStartRotation;
    }

    protected Vector3 GetAvoidanceFromOtherEnemies()
    {
        // Push away from close enemies.
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

        return avoidanceDirection;
    }
    protected void ClampTransformToLevelBounds()
    {
        // if the position is over the boundary, clamp it back to the boundary
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -GameManager.instance.levelBounds.x, GameManager.instance.levelBounds.x);
        pos.z = Mathf.Clamp(pos.z, -GameManager.instance.levelBounds.y, GameManager.instance.levelBounds.y);
        transform.position = pos;
    }
    
    public void SetHealthBarVisibility(bool visible)
    {
        healthBarUI.SetActive(visible);
    }

    protected void UpdateHealthText()
    {
        healthText.text = currentHealth.ToString();
    }

    public void TakeDamage(int damage)
    {
        // Take damage, die if at 0.
        currentHealth -= damage;
        UpdateHealthText();
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

    public virtual void ApplyKnockBack(Vector3 direction, float intensity)
    {
        if(_isKnockedBack) StopAllCoroutines();
        _isKnockedBack = true;
        StartCoroutine(KnockBackRoutine(direction * intensity));
    }
    
    // Create a coroutine that will move the enemy in the direction of the knockback for 0.4 seconds with the given intensity being the distance the enemy will move.
    protected virtual IEnumerator KnockBackRoutine(Vector3 knockBackVector)
    {
        float knockBackTime = 0.20f * knockBackVector.magnitude;
        float elapsedTime = 0f;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + knockBackVector * knockBackFactor;

        while (elapsedTime < knockBackTime)
        {
            // first we need to normalize the elapsed time.
            var normalizedTime = elapsedTime / knockBackTime;
            
            // Apply cubic easing-out function to the normalized time.
            normalizedTime = 1 - Mathf.Pow(1 - normalizedTime, 3);
            
            // Now we can lerp via the normalized time.
            transform.position = Vector3.Lerp(originalPosition, targetPosition, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _isKnockedBack = false;
    }
    
}
