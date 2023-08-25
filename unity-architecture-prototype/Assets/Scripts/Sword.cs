using UnityEngine;

public class Sword : MonoBehaviour
{
    public PlayerController parent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(parent.swordDamage);
                
                // direction is equal to the direction from the enemy to the player.
                var direction = parent.transform.position - enemy.transform.position;
                
                // Project on plane, reverse and normalize
                direction = Vector3.ProjectOnPlane(-direction, Vector3.up).normalized;
                
                enemy.ApplyKnockBack(direction, parent.swordKnockBackIntensity);
            }
        }
    }
}