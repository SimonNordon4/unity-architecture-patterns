using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    
    [Header("Prefabs")]
    public List<EnemySpawnBlock> enemySpawnBlocks = new();
    private EnemySpawnBlock _currentBlock;
    public GameObject spawnIndicatorPrefab;
    public Transform playerTarget;



    [Header("Stats")] 
    public float spawnRadius = 10f;
    
    [Header("Enemies")]
    public List<GameObject> enemies = new List<GameObject>();
    
    // Block data
    private bool _blocksCompleted = false;
    private float _elapsedBlockTime = 0f;
    private int _currentBlockIndex = 0;
    private int _currentBlockSpawnedEnemies = 0;
    private float[] _spawnTimings;

    // Will destroy all alive enemies.
    public void ResetEnemyManager()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
        StopAllCoroutines();
        _blocksCompleted = false;
        _elapsedBlockTime = 0f;
        _currentBlockIndex = 0;
        _currentBlockSpawnedEnemies = 0;
        _currentBlock = enemySpawnBlocks[0];
    }

    private void Start()
    {
        _currentBlock = enemySpawnBlocks[0];
        StopAllCoroutines();
        InitializeNewBlock();
    }

    private void InitializeNewBlock()
    {
        _elapsedBlockTime = 0f;
        _currentBlockSpawnedEnemies = 0;
        // Get the spawn timing of each enemy evaluated against the animation curve
        _spawnTimings = new float[_currentBlock.totalEnemies];
        for (var i = 0; i < _currentBlock.totalEnemies; i++)
        {
            _spawnTimings[i] = _currentBlock.spawnRateCurve.Evaluate((float) i / _currentBlock.totalEnemies) * _currentBlock.blockTime;
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGameActive == false) return;

        if (!_blocksCompleted)
        {
            _elapsedBlockTime += Time.deltaTime;

            if (_currentBlockSpawnedEnemies < _currentBlock.totalEnemies)
            {
                if (_elapsedBlockTime > _spawnTimings[_currentBlockSpawnedEnemies])
                {
                    // Spawn one the enemies inside the spawn chances based on their spawn chance
                    var totalSpawnChance = 0;
                    foreach (var enemySpawnChance in _currentBlock.enemySpawnActions)
                    {
                        totalSpawnChance += enemySpawnChance.spawnChance;
                    }
                
                    var randomSpawnChance = Random.Range(0, totalSpawnChance);
                    var currentSpawnChance = 0;
                
                    for (var i = 0; i < _currentBlock.enemySpawnActions.Count; i++)
                    {
                        var x = i;
                        currentSpawnChance += _currentBlock.enemySpawnActions[x].spawnChance;

                        if (randomSpawnChance >= currentSpawnChance)
                        {
                            Debug.Log("Less than spawn chance, skipping");
                            continue;
                        }
                        StartCoroutine(IndicateSpawn(_currentBlock.enemySpawnActions[x].enemyPrefab));
                        Debug.Log("Spawning Enemy");
                        _currentBlockSpawnedEnemies++;
                        break;
                    }
                }
            }
            

            // for (var i = 0; i < eliteSpawnTimings.Length; i++)
            // {
            //     if(Mathf.FloorToInt(_elapsedTime) % eliteSpawnTimings[i] == 0)
            //     {
            //         // Spawn Elite
            //     }
            // }
            
            if(_currentBlockSpawnedEnemies >= _currentBlock.totalEnemies)
            {
                // Spawn Boss
                Debug.Log("Spawning Boss");
                NextBlock();
            }
        }
        else
        {
            if(enemies.Count <= 0)
                gameManager.WinGame();
        }

    }

    private IEnumerator IndicateSpawn(GameObject enemyPrefab)
    {
        // select a random point on the circle
        var randomPoint = Random.insideUnitSphere.normalized * spawnRadius;
        randomPoint.y = enemyPrefab.transform.localScale.y;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, randomPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        
        // Suspend the coroutine until the game is active
        while (GameManager.instance.isGameActive == false)
        {
            yield return null;
        }
        
        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            yield break;
        }
        
        SpawnEnemy(enemyPrefab, randomPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        var newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().playerTarget = playerTarget;
        newEnemy.GetComponent<EnemyController>().enemyManager = this;
        enemies.Add(newEnemy);
    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public void NextBlock()
    {
        var currentIndex = enemySpawnBlocks.IndexOf(_currentBlock);

        if (currentIndex + 1 >= enemySpawnBlocks.Count)
        {
            _blocksCompleted = true;
            return;
        }
        
        _currentBlock = enemySpawnBlocks[currentIndex + 1];
        InitializeNewBlock();
    }
}