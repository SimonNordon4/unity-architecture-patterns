using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform playerTarget;
    
    public float spawnRate = 1f;
    private float _timeSinceLastSpawn;
    private List<GameObject> _enemies = new List<GameObject>();
    
    public float spawnRadius = 10f;

    private void Start()
    {
        SpawnEnemy();
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if(_timeSinceLastSpawn > spawnRate)
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        // select a random point on the circle
        var randomPoint = Random.insideUnitSphere.normalized * spawnRadius;
        randomPoint.y =1;
        var newEnemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().playerTarget = playerTarget;
        _enemies.Add(newEnemy);
        _timeSinceLastSpawn = 0f;
    }
}
