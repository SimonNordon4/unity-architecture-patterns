using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MoveBehaviour
    {
        TowardsPlayer,
        RandomLocation
    }
    
    [Header("References")]
    public EnemyManagerOld enemyManager;
    public bool isBoss = false;
    
    [Header("UI")]
    public GameObject healthBarUI;
    public TextMeshProUGUI healthText;
    protected Quaternion uiStartRotation;
    public TextMeshProUGUI damageText;
    public MoveBehaviour moveBehaviour = MoveBehaviour.TowardsPlayer;
    
    [Header("Movement")]
    public Transform playerTarget;
    public float moveSpeed = 5f;
    public float repulsionForce = 0.5f;
    public float knockBackFactor = 1f;
    public bool isUnstoppable = false;
    public bool isKnockedBack = false;
    protected Vector3 randomPosition = Vector3.zero;
    private float _radius = 0.5f;

    [Header("Health")] 
    public int currentHealth = 5;
    
    [Header("Attack")]
    public int damageAmount = 1;
    public float damageCooldown = 0.2f;
    protected float _timeSinceLastDamage;
    protected readonly List<Transform> _nearbyEnemies = new(4);
    
    [Header("Effects")]
    public ParticleSystem deathEffect;

    public GameObject spawnIndicator;
    
    [Header("Sounds")]
    public SoundDefinition deathSound;

    public SoundDefinition onHitSound;
    public SoundDefinition attackSound;

    private Coroutine damageTextCoroutine = null;
    public Coroutine knockBackCoroutine = null;

    protected virtual void Start()
    {
        // de-parent so we don't follow the enemy rotation.
        uiStartRotation = healthBarUI.transform.rotation;
        healthBarUI.SetActive(SettingsManager.instance.showEnemyHealthBars);
        UpdateHealthText();
        _radius = transform.localScale.x;
        randomPosition = new Vector3(Random.Range(GameManager.instance.levelBounds.x * -1, GameManager.instance.levelBounds.x), 0, Random.Range(GameManager.instance.levelBounds.y * -1, GameManager.instance.levelBounds.y));
    }

    protected virtual void Update()
    {
        if(GameManager.instance.isGameActive == false) return;
        
        if(isKnockedBack) return;

        switch (moveBehaviour)
        {
            case(MoveBehaviour.TowardsPlayer):
                TowardsPlayer();
                break;
            case(MoveBehaviour.RandomLocation):
                RandomLocation();
                break;
        }
        
        // Track Attack cooldown.
        _timeSinceLastDamage += Time.deltaTime;
        
        healthBarUI.transform.rotation = uiStartRotation;
    }

    protected virtual void TowardsPlayer()
    {
        if (playerTarget == null) return;
        var dir =  Vector3.ProjectOnPlane(playerTarget.position - transform.position,Vector3.up).normalized;
        var distance = Vector3.Distance(playerTarget.position, transform.position);
        if (distance < 0.5f)
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
    }

    protected virtual void RandomLocation()
    {
        var position = transform.position;
        var projectedPosition = new Vector3(position.x, 0, position.z);
        var dir =  Vector3.ProjectOnPlane(randomPosition - projectedPosition,Vector3.up).normalized;
        var distance = Vector3.Distance(randomPosition, projectedPosition);
        
        // tolerance is the radius of the enemy.
        var tolerance = transform.localScale.x * 0.5f;

        if (distance < tolerance + 0.5f) 
        {
            randomPosition = new Vector3(Random.Range(GameManager.instance.levelBounds.x * -1, GameManager.instance.levelBounds.x), 0, Random.Range(GameManager.instance.levelBounds.y * -1, GameManager.instance.levelBounds.y));
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
            var enemyRadius = enemy.localScale.x;
            var touchDistance = _radius + enemyRadius;
            if (distance < touchDistance)  // We don't want to divide by zero or a negative number
            {
                // normalize the distance.
                var normalizedDistance = distance / touchDistance;
                // We inverse the force so it's weak when far away, and very strong when close.
                var force = 1 - normalizedDistance;
                // make it REALLY weak when far away.
                var polynomialForce = force * force * force;
                avoidanceDirection -= enemyDir * polynomialForce;
            }
        }

        return avoidanceDirection;
    }
    protected void ClampTransformToLevelBounds()
    {
        // if the position is over the boundary, clamp it back to the boundary
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -GameManager.instance.levelBounds.x, GameManager.instance.levelBounds.x);
        pos.y = transform.localScale.y;
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
        AccountManager.instance.statistics.totalDamageDealt += damage;
        // Take damage, die if at 0.
        currentHealth -= damage;
        UpdateHealthText();
        if (currentHealth <= 0)
        {
            AudioManager.instance.PlaySound(deathSound);
            var pos = transform.position;
            var projectedPosition = new Vector3(pos.x, 0,pos.z);
            var dead = Instantiate(this.deathEffect, projectedPosition, Quaternion.identity);
            GameManager.instance.StartCoroutine(DestroyAfter(dead.gameObject));
            enemyManager.EnemyDied(gameObject);
        }
        else
        {
            AudioManager.instance.PlaySound(onHitSound);
            if(damageTextCoroutine != null) StopCoroutine(damageTextCoroutine);
            damageTextCoroutine = StartCoroutine(ShowDamageText(damage));
        }
    }

    private IEnumerator DestroyAfter(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // Add nearby enemies to list.
        if (other.CompareTag("Enemy"))
        {
            if(_nearbyEnemies.Count < 4)
                _nearbyEnemies.Add(other.transform);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        // Continuously damage the player.
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (_timeSinceLastDamage >= damageCooldown)
                {
                    
                    playerController.TakeDamage(damageAmount);
                    _timeSinceLastDamage = 0f;
                }

            }
        }
    }
    
    protected virtual void OnTriggerExit(Collider other)
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
        if (isUnstoppable) return;
        if(isKnockedBack && knockBackCoroutine != null) StopCoroutine(knockBackCoroutine);
        isKnockedBack = true;
        knockBackCoroutine = StartCoroutine(KnockBackRoutine(direction * intensity));
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
            
            // check if we're at the bounds of the level
            ClampTransformToLevelBounds();
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isKnockedBack = false;
    }
    
    protected virtual IEnumerator ShowDamageText(int damage)
    {

        damageText.text = damage.ToString();
        GameObject o = damageText.gameObject;
        o.SetActive(true);
        var elapsedTime = 0f;
        var t = o.transform;
        
        var startPosition = Vector3.right;
        var targetPosition = Vector3.up + Vector3.right * 1f;

        var startScale = Vector3.one * 0.25f;
        var targetScale = Vector3.one;
        
        while (elapsedTime < 0.6f)
        {
            elapsedTime += Time.deltaTime;
            var normalizedTime = elapsedTime / 0.4f;
            var quadraticTime = normalizedTime * normalizedTime;
            var inversedQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
            t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position, inversedQuadraticTime);
            t.localScale = Vector3.Lerp(startScale, targetScale, inversedQuadraticTime);
            yield return new WaitForEndOfFrame();
        }
        damageText.gameObject.SetActive(false);
        yield return null;
    }
    
}
