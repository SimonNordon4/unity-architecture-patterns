using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 10f;
    public int damage = 1;
    public float projectileLifetime = 5f;
    private float _timeAlive = 0f;

    // Update is called once per frame
    void Update()
    {
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
            Destroy(this.gameObject);
        }
    }
}
