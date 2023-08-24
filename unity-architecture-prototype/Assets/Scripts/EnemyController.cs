using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform playerTarget;
    public float moveSpeed = 5f;
    
    [Header("Attack")]
    public int damageAmount = 1;
    public float damageCooldown = 0.2f;
    private float _timeSinceLastDamage;

    private void Update()
    {
        // Move towards the player.
        if (playerTarget == null) return;
        var dir =  Vector3.ProjectOnPlane(playerTarget.position - transform.position,Vector3.up).normalized;

        if (dir.magnitude > 0)
        {
            transform.position += dir * (Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.LookRotation(dir);
        }
        
        // Attack cooldown timer.
        _timeSinceLastDamage += Time.deltaTime;
    }
    
    private void OnTriggerStay(Collider other)
    {
        // Continuously attack the player while in contact with them.
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
}
