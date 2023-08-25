using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 10f;
    public int damage = 1;
    public float projectileLifetime = 5f;
    private float _timeAlive = 0f;
    public float knockBackIntensity = 1;

    
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isGameActive == false) return;
        
        if(_timeAlive > projectileLifetime)
            Destroy(gameObject);
        
        _timeAlive += Time.deltaTime;
        
        transform.position += transform.forward * (projectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // get enemy controller component
            var enemyController = other.GetComponent<EnemyController>();
            enemyController.TakeDamage(damage);
            
            // We have to ensure we didn't just kill the enemy.
            if(enemyController != null)
                enemyController.ApplyKnockBack(transform.forward, knockBackIntensity);
            Destroy(this.gameObject);
        }
    }
}
