using System;
using System.Collections;
using System.Collections.Generic;
using Definitions;
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
    public float spawnRadius = 15f;
    
    [Header("Enemies")]
    public List<GameObject> enemies = new List<GameObject>();

    public int totalEnemiesKilled = 0;
    
    // Block data
    private bool _blocksCompleted = false;
    private float _elapsedBlockTime = 0f;
    private int _currentBlockIndex = 0;
    private int _currentBlockSpawnedEnemies = 0;
    private float[] _spawnTimings;
    private int _currentBlockAliveEnemies = 0;
    private bool _blockBossSpawned = false;

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
                        Debug.Log("Starting Spawn Action from Update");
                        StartCoroutine(StartSpawnAction(_currentBlock.enemySpawnActions[x]));
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
            
            // If we have spawned all enemies and the boss has not been spawned yet, spawn the boss.
            if(_currentBlockSpawnedEnemies >= _currentBlock.totalEnemies && _blockBossSpawned == false)
            {
                // Spawn Boss
                Debug.Log("Spawning Boss");
                if (_currentBlock.bossAction != null)
                {
                    StartCoroutine(StartSpawnAction(_currentBlock.bossAction));
                }
                _blockBossSpawned = true;
            }
            // spawning the boss will then increase the currentBlockSpawnEnemies, so once that is zero
            else if(_currentBlockAliveEnemies <= 0 && _blockBossSpawned == true)
            {
                NextBlock();
            }
        }
        else
        {
            if(_currentBlockAliveEnemies <= 0)
                gameManager.WinGame();
        }

    }

    private IEnumerator StartSpawnAction(EnemySpawnAction action)
    {
        // We need to pre populate the number of spawned enemies, otherwise it will continuously spawn enemies.
        // When we yield return null.
        _currentBlockSpawnedEnemies += action.numberOfEnemiesToSpawn;
        // Clamp the number of enemies to spawn to the total enemies in the block
        if(_currentBlockSpawnedEnemies > _currentBlock.totalEnemies)
            _currentBlockSpawnedEnemies = _currentBlock.totalEnemies;
        
        _currentBlockAliveEnemies += action.numberOfEnemiesToSpawn;
        
        Debug.Log("Starting enemy spawn action");
        var startPoint = Random.insideUnitSphere.normalized * spawnRadius;

        var lastSpawnPoint = startPoint;
        for (var i = 0; i < action.numberOfEnemiesToSpawn; i++)
        {
            // The first spawn is always on target.
            if (i == 0)
            {
                StartCoroutine(IndicateSpawn(action.enemyPrefab, startPoint));
                continue;
            }

            var delay = Random.Range(0.05f, 0.5f);

            lastSpawnPoint = Random.insideUnitSphere.normalized + lastSpawnPoint;
            yield return new WaitForSeconds(delay);
            StartCoroutine(IndicateSpawn(action.enemyPrefab, lastSpawnPoint));
        }



        yield return null;
    }

    private IEnumerator IndicateSpawn(GameObject enemyPrefab, Vector3 spawnPoint)
    {
        
        spawnPoint.y = enemyPrefab.transform.localScale.y;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, spawnPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        
        // Suspend the coroutine until the game is active
        while (GameManager.instance.isGameActive == false)
        {
            yield return null;
        }
        
        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            _currentBlockAliveEnemies--;
            yield break;
        }
        
        SpawnEnemy(enemyPrefab, spawnPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        var newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        var enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.playerTarget = playerTarget;
        enemyController.enemyManager = this;
        
        var enemyHealth = enemyController.currentHealth * _currentBlock.healthMultiplier;
        var enemyHealthVariance = Random.Range(-_currentBlock.healthMultiplierTolerance, _currentBlock.healthMultiplierTolerance);
        enemyHealth = enemyHealth + enemyHealth * enemyHealthVariance;
        enemyController.currentHealth = Mathf.RoundToInt(enemyHealth);
        
        var enemyDamage = enemyController.damageAmount * _currentBlock.damageMultiplier;
        var enemyDamageVariance = Random.Range(-_currentBlock.damageMultiplierTolerance, _currentBlock.damageMultiplierTolerance);
        enemyDamage = enemyDamage + enemyDamage * enemyDamageVariance;
        enemyController.damageAmount = Mathf.RoundToInt(enemyDamage);
        
        enemies.Add(newEnemy);
    }

    public void EnemyDied(GameObject enemy)
    {
        _currentBlockAliveEnemies--;
        totalEnemiesKilled++;
        enemies.Remove(enemy);
        GameManager.instance.OnEnemyDied(enemy);
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