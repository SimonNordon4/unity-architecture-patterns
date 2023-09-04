using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Stats")] 
    public float spawnRadius = 15f;
    public EnemySpawnRound enemySpawnRound;

    private EnemySpawnBlock _currentBlock;
    private int _blockIndex;
    
    private List<EnemySpawnWave> _currentSpawnWaves = new();
    private EnemySpawnWave _currentWave;
    
    // for UI purposes.
    private int totalWaves = 0;
    private int thisWave = 0;

    [Header("Enemies")] public readonly List<GameObject> enemies = new List<GameObject>();

    public int totalEnemiesKilled = 0;
    private Vector3 _lastSpawnPoint;

    // Block data
    private float _elapsedWaveTime = 0f;
    private int _currentWaveSpawnedEnemies = 0;
    private int _currentWaveAliveEnemies = 0;
    private float[] _spawnTimings;
    private int _waveIndex = 0;

    public EnemySpawnPhase currentPhase = EnemySpawnPhase.Normal;

    // Boss data
    private readonly List<GameObject> bossEnemies = new List<GameObject>();
    private int _bossEnemiesCount;
    private Vector3 _positionOfLastBossDeath = Vector3.zero;

    // Chest data
    private GameObject _bossChest;
    
    // Wave Data
    public readonly List<WaveRuntimeData> WaveDatas = new();
    private WaveRuntimeData _currentWaveData;


    // Will destroy all alive enemies.
    public void ResetEnemyManager()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }

        enemies.Clear();
        StopAllCoroutines();
        _elapsedWaveTime = 0f;
        _currentWaveAliveEnemies = 0;
        _currentWaveSpawnedEnemies = 0;


        foreach (var enemy in bossEnemies)
        {
            Destroy(enemy);
        }
        
        bossEnemies.Clear();
        _bossEnemiesCount = 0;
        _positionOfLastBossDeath = Vector3.zero;
        _bossChest = null;
        _waveIndex = 0;
        totalEnemiesKilled = 0;
        currentPhase = EnemySpawnPhase.Normal;
        _currentBlock = enemySpawnRound.enemySpawnBlocks[0];
        _currentSpawnWaves = _currentBlock.spawnWaves;
        _blockIndex = 0;
        _currentWave = _currentSpawnWaves[0];
        InitializeNewWave();
        
        totalWaves = enemySpawnRound.enemySpawnBlocks.Sum(block => block.spawnWaves.Count);
        thisWave = 1;
        gameManager.waveText.text = "Wave " + thisWave + "/" + totalWaves;
        
        _currentWaveData = new WaveRuntimeData
        {
            startTime = gameManager.roundTime
        };
        WaveDatas.Clear();
        WaveDatas.Add(_currentWaveData);
        
    }

    private void Start()
    {
        _currentBlock = enemySpawnRound.enemySpawnBlocks[0];
        _currentWave = _currentBlock.spawnWaves[0];
        StopAllCoroutines();
        InitializeNewWave();
        totalWaves = enemySpawnRound.enemySpawnBlocks.Sum(block => block.spawnWaves.Count);
        thisWave = 1;
        gameManager.waveText.text = "Wave " + thisWave + "/" + totalWaves;
    }

    private void InitializeNewWave()
    {
        _elapsedWaveTime = 0f;
        _currentWaveSpawnedEnemies = 0;
        _currentWaveAliveEnemies = 0;
        currentPhase = EnemySpawnPhase.Normal;

        // Get the spawn timing of each enemy evaluated against the animation curve
        _spawnTimings = new float[_currentWave.totalEnemies];
        for (var i = 0; i < _currentWave.totalEnemies; i++)
        {
            _spawnTimings[i] = _currentWave.spawnRateCurve.Evaluate((float)i / _currentWave.totalEnemies) *
                               _currentWave.blockTime;
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
                if (_bossEnemiesCount <= 0 && _currentWaveAliveEnemies <= 0)
                {
                    currentPhase = EnemySpawnPhase.BossDead;
                }
                break;
            case(EnemySpawnPhase.BossDead):
                if (_waveIndex >= _currentSpawnWaves.Count - 1 && _blockIndex >= enemySpawnRound.enemySpawnBlocks.Count)
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
                NextWave();
                break;
        }
    }

    #region Spawn Phases

    private void HandleNormalEnemies()
    {
        
        // Enemy Spawn Rate now doubles the spawn time of the block up until the next block.
        if ((int)gameManager.enemySpawnRate.value > _blockIndex)
        {
            _elapsedWaveTime += Time.deltaTime * 2;
        }
        else
        {
            _elapsedWaveTime += Time.deltaTime;
        }
            

        if (_elapsedWaveTime > _currentWave.blockTime)
        {
            currentPhase = EnemySpawnPhase.SpawnBoss;
            return;
        }
        
        // We don't want to continue spawning after we've reached max enemies.
        if(_currentWaveSpawnedEnemies >= _currentWave.totalEnemies)
            return;
        
        if (_elapsedWaveTime > _spawnTimings[_currentWaveSpawnedEnemies])
        {
            // Spawn one the enemies inside the spawn chances based on their spawn chance
            var totalSpawnChance = 0;
            foreach (var enemySpawnChance in _currentWave.enemySpawnActions)
            {
                totalSpawnChance += enemySpawnChance.spawnWeight;
            }

            var randomSpawnChance = Random.Range(0, totalSpawnChance);
            var currentSpawnChance = 0;

            for (var i = 0; i < _currentWave.enemySpawnActions.Count; i++)
            {
                var x = i;
                currentSpawnChance += _currentWave.enemySpawnActions[x].spawnWeight;

                if (randomSpawnChance >= currentSpawnChance)
                {
                    continue;
                }

                var spawnAction = _currentWave.enemySpawnActions[x];
                _currentWaveAliveEnemies += spawnAction.numberOfEnemiesToSpawn;
                _currentWaveSpawnedEnemies += spawnAction.numberOfEnemiesToSpawn;
                
                StartCoroutine(StartSpawnAction(spawnAction));
                break;
            }
        }
    }

    private void HandleSpawnBoss()
    {
        StartCoroutine(StartBossSpawnAction(_currentWave.bossAction));
        currentPhase = EnemySpawnPhase.BossAlive;
    }

    #endregion

    #region Spawn Normal Enemy

    private IEnumerator StartSpawnAction(EnemySpawnAction action)
    {
        // Clamp the number of enemies to spawn to the total enemies in the block
        if (_currentWaveSpawnedEnemies > _currentWave.totalEnemies)
            _currentWaveSpawnedEnemies = _currentWave.totalEnemies;

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
            _currentWaveAliveEnemies--;
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

        enemyController.currentHealth = Mathf.RoundToInt(enemyController.currentHealth * Random.Range(_currentBlock.healthMultiplier.x, _currentBlock.healthMultiplier.y));
        enemyController.damageAmount = Mathf.RoundToInt(enemyController.damageAmount * Random.Range(_currentBlock.damageMultiplier.x, _currentBlock.damageMultiplier.y));

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
            StartCoroutine(IndicateBossSpawn(action, lastSpawnPoint));
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
            _currentWaveAliveEnemies--;
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
        enemyController.currentHealth = Mathf.RoundToInt(enemyController.currentHealth * Random.Range(_currentBlock.healthMultiplier.x, _currentBlock.healthMultiplier.y));
        enemyController.damageAmount = Mathf.RoundToInt(enemyController.damageAmount * Random.Range(_currentBlock.damageMultiplier.x, _currentBlock.damageMultiplier.y));
        enemyController.isBoss = true;

        enemies.Add(newEnemy);
        bossEnemies.Add(newEnemy);
    }

    private void SpawnBossChest()
    {
        Chest chestController;
        // if max tier is 3 spawn a medium chest.
        var projectedPosition = new Vector3(_positionOfLastBossDeath.x, 0, _positionOfLastBossDeath.z);
        
        if (_currentBlock.bossChestTier.y <= 3)
        {
            _bossChest = Instantiate(mediumChestPrefab, projectedPosition, Quaternion.identity);
            chestController = _bossChest.GetComponent<Chest>();
        }
        else
        {
            _bossChest = Instantiate(largeChestPrefab, projectedPosition, Quaternion.identity);
            chestController = _bossChest.GetComponent<Chest>();
        }

        chestController.minTier = _currentBlock.bossChestTier.x;
        chestController.maxTier = _currentBlock.bossChestTier.y;
        chestController.options = _currentBlock.bossChestChoices;
    }

    #endregion

    public void EnemyDied(GameObject enemy)
    {
        if (enemy.GetComponent<EnemyController>().isBoss)
        {
            Debug.Log("Boss died");
            _positionOfLastBossDeath = enemy.transform.position;
            _bossEnemiesCount--;
            bossEnemies.Remove(enemy);
            enemies.Remove(enemy);
            totalEnemiesKilled++;
            GameManager.instance.OnBossEnemyDied(enemy);
            Destroy(enemy);
            return;
        }
        _currentWaveData.currentGold += _currentBlock.goldMultiplier;
        _currentWaveAliveEnemies--;
        totalEnemiesKilled++;
        enemies.Remove(enemy);
        GameManager.instance.OnEnemyDied(enemy);
        Destroy(enemy);
    }

    public void NextWave()
    {
        _waveIndex++;
        // to get the bonus gold we need to get the time difference between the start and end of the block.
        var timeDifference = gameManager.roundTime - _currentWaveData.startTime;
        // assuming max bonus is the duration of the wave, and at double time we have no bonus.
        var bonusPercentage = _currentWave.blockTime * 2 - timeDifference;
        // clamp the bonus percentage to 0 - 100
        bonusPercentage = Mathf.Clamp(bonusPercentage, 0, 100);
        // get the bonus gold based on the percentage.
        _currentWaveData.bonusGold = _currentWaveData.currentGold * (bonusPercentage / 100);
        
        // If we've reached the end of the block, go to the next block
        if (_waveIndex >= _currentSpawnWaves.Count)
        {
            BlockBeaten();
            _blockIndex++;
            if (_blockIndex >= enemySpawnRound.enemySpawnBlocks.Count)
            {
                GameManager.instance.WinGame();
                return;
            }

            // create a new block data.
            _currentBlock = enemySpawnRound.enemySpawnBlocks[_blockIndex];
            _currentSpawnWaves = _currentBlock.spawnWaves;
            _waveIndex = 0;
        }
        
        thisWave++;
        gameManager.waveText.text = "Wave " + thisWave + "/" + totalWaves;
        
        _currentWave = _currentSpawnWaves[_waveIndex];
        _currentWaveData = new WaveRuntimeData
        {
            startTime = gameManager.roundTime
        };
        WaveDatas.Add(_currentWaveData);
        InitializeNewWave();
    }

    public void BlockBeaten()
    {
        
    }

    public class WaveRuntimeData
    {
        public float startTime;
        public float finishTime;
        public float currentGold = 0;
        public float bonusGold = 0;
    }
}