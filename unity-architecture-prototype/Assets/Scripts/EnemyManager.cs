using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject spawnIndicatorPrefab;
    public Transform playerTarget;

    [Header("Stats")] 
    public int totalEnemies = 20;
    private int _spawnedEnemies = 0;
    private int _aliveEnemies = 0;
    public float spawnRate = 4f;

    private float _timeSinceLastSpawn;
    
    public float spawnRadius = 10f;
    
    [Header("Enemies")]
    public List<GameObject> enemies = new List<GameObject>();
    
    private float CalculateSpawnInterval()
    {
        return spawnRate / Mathf.Sqrt(_spawnedEnemies + 1);
    }

    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(IndicateSpawn());
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn > CalculateSpawnInterval())
        {
            StartCoroutine(IndicateSpawn());
            _timeSinceLastSpawn = 0f;
        }
    }

    private IEnumerator IndicateSpawn()
    {
        // select a random point on the circle
        var randomPoint = Random.insideUnitSphere.normalized * spawnRadius;
        randomPoint.y =1;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, randomPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        
        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            yield break;
        }
        
        SpawnEnemy(randomPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnEnemy(Vector3 position)
    {
        var newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().playerTarget = playerTarget;
        newEnemy.GetComponent<EnemyController>().enemyManager = this;
        _spawnedEnemies++;
        enemies.Add(newEnemy);

    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
}