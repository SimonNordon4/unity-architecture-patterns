using System;
using System.Collections;
using System.Collections.Generic;
using Definitions;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemySpawnPhase
{
    None,
    Normal,
    SpawnBoss,
    BossAlive,
    BossDead,
    SpawnChest,
    ChestAlive,
    ChestCollected
}

public class EnemyManager : MonoBehaviour
{
    [Header("References")] public GameManager gameManager;
    public Transform playerTarget;

    [Header("Prefabs")] public GameObject spawnIndicatorPrefab;
    public GameObject mediumChestPrefab;
    public GameObject largeChestPrefab;

    [Header("Stats")] public float spawnRadius = 15f;
    public List<EnemySpawnBlock> enemySpawnBlocks = new();
    private EnemySpawnBlock _currentBlock;

    [Header("Enemies")] public List<GameObject> enemies = new List<GameObject>();

    public int totalEnemiesKilled = 0;
    private Vector3 _lastSpawnPoint;

    // Block data
    private float _elapsedBlockTime = 0f;
    private int _currentBlockSpawnedEnemies = 0;
    private int _currentBlockAliveEnemies = 0;
    private float[] _spawnTimings;
    private int _blockIndex = 0;

    public EnemySpawnPhase currentPhase = EnemySpawnPhase.Normal;

    // Boss data
    private readonly List<GameObject> bossEnemies = new List<GameObject>();
    private int _bossEnemiesCount;
    private Vector3 _positionOfLastBossDeath = Vector3.zero;

    // Chest data
    private GameObject _bossChest;


    // Will destroy all alive enemies.
    public void ResetEnemyManager()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }

        enemies.Clear();
        StopAllCoroutines();
        _elapsedBlockTime = 0f;
        _currentBlockAliveEnemies = 0;
        _currentBlockSpawnedEnemies = 0;


        foreach (var enemy in bossEnemies)
        {
            Destroy(enemy);
        }
        
        bossEnemies.Clear();
        _bossEnemiesCount = 0;
        _positionOfLastBossDeath = Vector3.zero;
        _bossChest = null;
        _blockIndex = 0;
        totalEnemiesKilled = 0;
        currentPhase = EnemySpawnPhase.Normal;
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
        _currentBlockAliveEnemies = 0;
        currentPhase = EnemySpawnPhase.Normal;
        // Get the spawn timing of each enemy evaluated against the animation curve
        _spawnTimings = new float[_currentBlock.totalEnemies];
        for (var i = 0; i < _currentBlock.totalEnemies; i++)
        {
            _spawnTimings[i] = _currentBlock.spawnRateCurve.Evaluate((float)i / _currentBlock.totalEnemies) *
                               _currentBlock.blockTime;
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGameActive == false) return;

        switch (currentPhase)
        {
            case (EnemySpawnPhase.Normal):
                HandleNormalEnemies();
                break;
            case (EnemySpawnPhase.SpawnBoss):
                HandleSpawnBoss();
                break;
            case(EnemySpawnPhase.BossAlive):
                if (_bossEnemiesCount <= 0 && _currentBlockAliveEnemies <= 0)
                {
                    currentPhase = EnemySpawnPhase.BossDead;
                }
                break;
            case(EnemySpawnPhase.BossDead):
                if (_blockIndex >= enemySpawnBlocks.Count - 1)
                {
                    GameManager.instance.WinGame();
                    return;
                }
                currentPhase = EnemySpawnPhase.SpawnChest;
                break;
            case (EnemySpawnPhase.SpawnChest):
                SpawnBossChest();
                currentPhase = EnemySpawnPhase.ChestAlive;
                break;
            case(EnemySpawnPhase.ChestAlive):
                if (_bossChest == null)
                {
                    currentPhase = EnemySpawnPhase.ChestCollected;
                }
                break;
            case (EnemySpawnPhase.ChestCollected):
                NextBlock();
                break;
        }
    }

    #region Spawn Phases

    private void HandleNormalEnemies()
    {
        _elapsedBlockTime += Time.deltaTime;

        if (_elapsedBlockTime > _currentBlock.blockTime)
        {
            Debug.Log("Block time exceeded, spawning boss");
            currentPhase = EnemySpawnPhase.SpawnBoss;
            return;
        }
        
        // We don't want to continue spawning after we've reached max enemies.
        if(_currentBlockSpawnedEnemies >= _currentBlock.totalEnemies)
            return;
        
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
                    continue;
                }

                var spawnAction = _currentBlock.enemySpawnActions[x];
                _currentBlockAliveEnemies += spawnAction.numberOfEnemiesToSpawn;
                _currentBlockSpawnedEnemies += spawnAction.numberOfEnemiesToSpawn;
                
                StartCoroutine(StartSpawnAction(spawnAction));
                break;
            }
        }
    }

    private void HandleSpawnBoss()
    {
        _bossEnemiesCount = _currentBlock.bossAction.numberOfEnemiesToSpawn;
        StartCoroutine(StartBossSpawnAction(_currentBlock.bossAction));
        currentPhase = EnemySpawnPhase.BossAlive;
    }

    #endregion

    #region Spawn Normal Enemy

    private IEnumerator StartSpawnAction(EnemySpawnAction action)
    {
        // Clamp the number of enemies to spawn to the total enemies in the block
        if (_currentBlockSpawnedEnemies > _currentBlock.totalEnemies)
            _currentBlockSpawnedEnemies = _currentBlock.totalEnemies;

        var startPoint = Random.insideUnitSphere.normalized * spawnRadius;

        var lastSpawnPoint = startPoint;
        for (var i = 0; i < action.numberOfEnemiesToSpawn; i++)
        {
            // The first spawn is always on target.
            if (i == 0)
            {
                StartCoroutine(IndicateSpawn(action, startPoint));
                continue;
            }

            var delay = Random.Range(0.05f, 0.5f);

            lastSpawnPoint = Random.insideUnitSphere.normalized + lastSpawnPoint;
            yield return new WaitForSeconds(delay);
            StartCoroutine(IndicateSpawn(action, lastSpawnPoint));
        }
        yield return null;
    }

    private IEnumerator IndicateSpawn(EnemySpawnAction enemyAction, Vector3 spawnPoint)
    {
        spawnPoint.y = enemyAction.enemyPrefab.transform.localScale.y;
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
            // Cancelling a spawn is assumed as killing an enemy.
            _currentBlockAliveEnemies--;
            yield break;
        }

        SpawnEnemy(enemyAction, spawnPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnEnemy(EnemySpawnAction enemyAction, Vector3 position)
    {
        var newEnemy = Instantiate(enemyAction.enemyPrefab, position, Quaternion.identity);
        var enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.playerTarget = playerTarget;
        enemyController.enemyManager = this;

        enemyController.currentHealth = enemyAction.health;
        enemyController.damageAmount = enemyAction.damage;

        enemies.Add(newEnemy);
    }

    #endregion

    #region Spawn Boss Enemy

    private IEnumerator StartBossSpawnAction(EnemySpawnAction action)
    {
        var startPoint = Random.insideUnitSphere.normalized * spawnRadius;
        
        _bossEnemiesCount = action.numberOfEnemiesToSpawn;

        var lastSpawnPoint = startPoint;
        for (var i = 0; i < action.numberOfEnemiesToSpawn; i++)
        {
            // The first spawn is always on target.
            if (i == 0)
            {
                StartCoroutine(IndicateBossSpawn(action, startPoint));
                continue;
            }

            var delay = Random.Range(0.05f, 0.5f);

            lastSpawnPoint = Random.insideUnitSphere.normalized + lastSpawnPoint;
            yield return new WaitForSeconds(delay);
            StartCoroutine(IndicateSpawn(action, lastSpawnPoint));
        }

        yield return null;
    }

    private IEnumerator IndicateBossSpawn(EnemySpawnAction enemyAction, Vector3 spawnPoint)
    {
        spawnPoint.y = enemyAction.enemyPrefab.transform.localScale.y;
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
            // Cancelling a spawn is assumed as killing an enemy.
            _currentBlockAliveEnemies--;
            yield break;
        }

        SpawnBossEnemy(enemyAction, spawnPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnBossEnemy(EnemySpawnAction enemyAction, Vector3 position)
    {
        var newEnemy = Instantiate(enemyAction.enemyPrefab, position, Quaternion.identity);
        var enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.playerTarget = playerTarget;
        enemyController.enemyManager = this;

        enemyController.currentHealth = enemyAction.health;
        enemyController.damageAmount = enemyAction.damage;
        enemyController.isBoss = true;

        enemies.Add(newEnemy);
        bossEnemies.Add(newEnemy);
    }

    private void SpawnBossChest()
    {
        Chest chestController;
        // if max tier is 3 spawn a medium chest.
        if (_currentBlock.bossChestTier <= 3)
        {
            _bossChest = Instantiate(mediumChestPrefab, _positionOfLastBossDeath, Quaternion.identity);
            chestController = _bossChest.GetComponent<Chest>();
        }
        else
        {
            _bossChest = Instantiate(largeChestPrefab, _positionOfLastBossDeath, Quaternion.identity);
            chestController = _bossChest.GetComponent<Chest>();
        }

        chestController.minTier = _currentBlock.bossChestTier;
        chestController.maxTier = _currentBlock.bossChestItems;
    }

    #endregion

    public void EnemyDied(GameObject enemy)
    {
        if (enemy.GetComponent<EnemyController>().isBoss)
        {
            _positionOfLastBossDeath = enemy.transform.position;
            _bossEnemiesCount--;
            bossEnemies.Remove(enemy);
            enemies.Remove(enemy);
            totalEnemiesKilled++;
            GameManager.instance.OnBossEnemyDied(enemy);
            Destroy(enemy);
            return;
        }

        _currentBlockAliveEnemies--;
        totalEnemiesKilled++;
        enemies.Remove(enemy);
        GameManager.instance.OnEnemyDied(enemy);
        Destroy(enemy);
    }

    public void NextBlock()
    {
        _blockIndex++;
        _currentBlock = enemySpawnBlocks[_blockIndex];
        InitializeNewBlock();
    }
}