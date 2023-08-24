using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject spawnIndicatorPrefab;
    public Transform playerTarget;
    
    public float spawnRate = 1f;
    private float _timeSinceLastSpawn;
    private List<GameObject> _enemies = new List<GameObject>();
    
    public float spawnRadius = 10f;

    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(IndicateSpawn());
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn > spawnRate)
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

    public void CancelSpawn(int spawnId)
    {
        
    }

    private void SpawnEnemy(Vector3 position)
    {
        var newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().playerTarget = playerTarget;
        _enemies.Add(newEnemy);
    }
}