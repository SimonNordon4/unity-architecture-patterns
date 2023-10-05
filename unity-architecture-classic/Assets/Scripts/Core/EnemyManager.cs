using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Classic.Game;
using Classic.Items;
using Definitions;
using UnityEngine;
using UnityEngine.Events;
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
    
    
    [Header("Dependencies")] 
    public ChestSpawner chestSpawner;

    public SettingsManager settings;

    public RoundTimer timer;
    public Level level;
    public GameState gameState;
    public UnityEvent<Vector3> onEnemyDied = new ();
    public UnityEvent<Vector3> onBossDied = new ();
    public UnityEvent<int> onDamageTaken = new ();
    
    [Header("References")] 
    public Transform playerTarget;

    [Header("Stats")] 
    public float spawnRadius = 15f;
    public EnemySpawnRound enemySpawnRound;

    private EnemySpawnBlock _currentBlock;
    private int _blockIndex;
    
    private List<EnemySpawnWave> _currentSpawnWaves = new();
    private EnemySpawnWave _currentWave;
    
    // for UI purposes.
    private int _totalWaves;
    private int _thisWave;

    [Header("Enemies")] 
    public readonly List<GameObject> enemies = new List<GameObject>();
    public int totalEnemiesKilled = 0;

    [Header("Spawn Rate")] 
    public float[] penaltySpawnRates;
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

    private void OnEnable()
    {
        gameState.onGameStart.AddListener(ResetEnemyManager);
    }
    
    private void OnDisable()
    {
        gameState.onGameStart.RemoveListener(ResetEnemyManager);
    }


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
        
        _totalWaves = enemySpawnRound.enemySpawnBlocks.Sum(block => block.spawnWaves.Count);
        _thisWave = 1;
        
        _currentWaveData = new WaveRuntimeData
        {
            startTime = timer.roundTime
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
        _totalWaves = enemySpawnRound.enemySpawnBlocks.Sum(block => block.spawnWaves.Count);
        _thisWave = 1;
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
        if (gameState.currentState != GameStateEnum.Active) return;

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
                    gameState.WinGame();
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

    private float CalculateSpawnRatePenalty()
    {
        // We want the enemies to spawn A LOT faster if the player is killing them very frequently
        // until we've once again reached the ideal enemies alive count.

        // Do not apply a spawn rate penalty if there's an ideal number of enemies.
        if (_currentWaveAliveEnemies > _currentWave.idealEnemiesAlive)
        {
            return 1f;
        }
        
        // assuming 3 enemies alive is the ideal number.
        // [6,5,4,3,2,1,0,-1]
        var handicap = _currentWave.idealEnemiesAlive + _currentWave.decay - _currentWaveSpawnedEnemies;
        if (handicap < 0) handicap = 0;

        var index = _currentWave.idealEnemiesAlive - _currentWaveAliveEnemies - handicap;
        index = Mathf.Clamp(index, -1, penaltySpawnRates.Length - 1);

        if (index == -1)
        {
            return 1f;
        }
        
        var penalty = penaltySpawnRates[index];
        
        return penalty;

    }

    private void HandleNormalEnemies()
    {
        _elapsedWaveTime += Time.deltaTime * CalculateSpawnRatePenalty();

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
        foreach (var action in _currentWave.eliteAction)
        {
            StartCoroutine(StartBossSpawnAction(action));
        }
        
        currentPhase = EnemySpawnPhase.BossAlive;
    }

    #endregion

    #region Spawn Normal Enemy

    private IEnumerator StartSpawnAction(EnemySpawnAction action)
    {
        // Clamp the number of enemies to spawn to the total enemies in the block
        if (_currentWaveSpawnedEnemies > _currentWave.totalEnemies)
            _currentWaveSpawnedEnemies = _currentWave.totalEnemies;

        var t = Random.value;
        float angle = t * Mathf.PI * 2;
        
        var distance = (1 - Mathf.Pow(t, 2)) * spawnRadius;
        
        var startPoint = new Vector3(MathF.Cos(angle),0f,MathF.Sin(angle)) * distance;

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
        spawnPoint = new Vector3(spawnPoint.x, 0f, spawnPoint.z);
        var spawnIndicatorPrefab = enemyAction.enemyPrefab.GetComponent<EnemyController>().spawnIndicator;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, spawnPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);

        // Suspend the coroutine until the game is active
        while (gameState.currentState != GameStateEnum.Active)
        {
            yield return null;
        }

        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            // Cancelling a spawn is assumed as killing an enemy.
            _currentWaveAliveEnemies--;
            totalEnemiesKilled++;
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
        enemyController.level = level;
        enemyController.gameState = gameState;
        enemyController.settings = settings;

        enemyController.currentHealth = Mathf.RoundToInt(enemyController.currentHealth * Random.Range(_currentWave.healthMultiplier.x, _currentWave.healthMultiplier.y));
        enemyController.damageAmount = Mathf.RoundToInt(enemyController.damageAmount * Random.Range(_currentWave.damageMultiplier.x, _currentWave.damageMultiplier.y));

        enemies.Add(newEnemy);
    }

    #endregion

    #region Spawn Boss Enemy

    private IEnumerator StartBossSpawnAction(EnemySpawnAction action)
    {
        var t = Random.value;
        float angle = t * Mathf.PI * 2;
        
        var distance = (1 - Mathf.Pow(t, 2)) * spawnRadius;
        
        var startPoint = new Vector3(MathF.Cos(angle),0f,MathF.Sin(angle)) * distance;
        
        _bossEnemiesCount += action.numberOfEnemiesToSpawn;

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
        spawnPoint = new Vector3(spawnPoint.x, 0f, spawnPoint.z);
        var spawnIndicatorPrefab = enemyAction.enemyPrefab.GetComponent<EnemyController>().spawnIndicator;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, spawnPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);

        // Suspend the coroutine until the game is active
        while (gameState.currentState != GameStateEnum.Active)
        {
            yield return null;
        }

        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            // Cancelling a spawn is assumed as killing an enemy.
            _currentWaveAliveEnemies--;
            totalEnemiesKilled++;
            _bossEnemiesCount--; 
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
        enemyController.currentHealth = Mathf.RoundToInt(enemyController.currentHealth * Random.Range(_currentWave.healthMultiplier.x, _currentWave.healthMultiplier.y));
        enemyController.damageAmount = Mathf.RoundToInt(enemyController.damageAmount * Random.Range(_currentWave.damageMultiplier.x, _currentWave.damageMultiplier.y));
        enemyController.isBoss = true;
        enemyController.level = level;
        enemyController.gameState = gameState;
        enemyController.settings = settings;

        enemies.Add(newEnemy);
        bossEnemies.Add(newEnemy);
    }

    private void SpawnBossChest()
    {
        Chest chestController;
        // if max tier is 3 spawn a medium chest.
        var projectedPosition = new Vector3(_positionOfLastBossDeath.x, 0.5f, _positionOfLastBossDeath.z);
        
        if (_currentWave.bossChestTier.y <= 3)
        {
            _bossChest = chestSpawner.SpawnChest(ChestType.Medium, projectedPosition).gameObject;
        }
        else
        {
            _bossChest = chestSpawner.SpawnChest(ChestType.Large, projectedPosition).gameObject;
        }
    }

    #endregion

    public void EnemyDied(GameObject enemy)
    {
        Debug.Log("Enemy died 1");
        if (enemy.GetComponent<EnemyController>().isBoss)
        {
            Debug.Log("Boss died");
            _positionOfLastBossDeath = enemy.transform.position;
            _bossEnemiesCount--;
            bossEnemies.Remove(enemy);
            enemies.Remove(enemy);
            totalEnemiesKilled++;
            Debug.Log("Boss Enemies alive: " + _bossEnemiesCount);

            Destroy(enemy);
            onBossDied.Invoke(enemy.transform.position);
            return;
        }

        if (_currentWaveData == null)
        {
            Debug.LogError("Current wave data is null!");
        }
        _currentWaveData.currentGold += _currentBlock.goldMultiplier;
        _currentWaveAliveEnemies--;
        totalEnemiesKilled++;
        
        enemies.Remove(enemy);

        onEnemyDied.Invoke(enemy.transform.position);
        Destroy(enemy);
    }

    public void NextWave()
    {
        _waveIndex++;
        // to get the bonus gold we need to get the time difference between the start and end of the block.
        var timeDifference = timer.roundTime - _currentWaveData.startTime;
        // assuming max bonus is the duration of the wave, and at double time we have no bonus.
        var bonusPercentage = _currentWave.blockTime * 2 - timeDifference;
        // clamp the bonus percentage to 0 - 100
        bonusPercentage = Mathf.Clamp(bonusPercentage, 0, 100);
        // get the bonus gold based on the percentage.
        _currentWaveData.bonusGold = _currentWaveData.currentGold * (bonusPercentage / 100);
        
        // If we've reached the end of the block, go to the next block
        if (_waveIndex >= _currentSpawnWaves.Count)
        {
            // // check achievements.
            // switch (_blockIndex)
            // {
            //     case (0):
            //         AccountManager.instance.AchievementUnlocked( AccountManager.instance.achievementSave.achievements
            //             .First(x=>x.id == AchievementName.BeatRound1));
            //         break;
            //     case (1):
            //         AccountManager.instance.AchievementUnlocked( AccountManager.instance.achievementSave.achievements
            //             .First(x=>x.id == AchievementName.BeatRound2));
            //         break;
            //     case (2):
            //         AccountManager.instance.AchievementUnlocked( AccountManager.instance.achievementSave.achievements
            //             .First(x=>x.id == AchievementName.BeatRound3));
            //         break;
            //     case (3):
            //         AccountManager.instance.AchievementUnlocked( AccountManager.instance.achievementSave.achievements
            //             .First(x=>x.id == AchievementName.BeatRound4));
            //         break;
            //     case (4):
            //         AccountManager.instance.AchievementUnlocked( AccountManager.instance.achievementSave.achievements
            //             .First(x=>x.id == AchievementName.BeatRound5));
            //         break;
            // }
            
            
            _blockIndex++;
            if (_blockIndex >= enemySpawnRound.enemySpawnBlocks.Count)
            {
                gameState.WinGame();
                return;
            }

            // create a new block data.
            _currentBlock = enemySpawnRound.enemySpawnBlocks[_blockIndex];
            _currentSpawnWaves = _currentBlock.spawnWaves;
            _waveIndex = 0;
            
            
        }
        
        _thisWave++;
        
        _currentWave = _currentSpawnWaves[_waveIndex];
        _currentWaveData = new WaveRuntimeData
        {
            startTime = timer.roundTime
        };
        WaveDatas.Add(_currentWaveData);
        InitializeNewWave();
    }

    public EnemySpawnWave GetCurrentWave()
    {
        return _currentWave;
    }

    public EnemySpawnWave GetNextWave()
    {
        Debug.Log($"_waveIndex: {_waveIndex}");
        Debug.Log($"_currentSpawnWaves.Count: {_currentSpawnWaves.Count}");
        
        // If we've reached the end of the block, go to the next block
        if (_waveIndex >= _currentSpawnWaves.Count - 1)
        {
            Debug.Log("Getting wave in next block.");
            var nextIndex = _blockIndex + 1;
            if (nextIndex >= enemySpawnRound.enemySpawnBlocks.Count)
            {
                return _currentSpawnWaves[^1];
            }
            var block = enemySpawnRound.enemySpawnBlocks[nextIndex];
            return block.spawnWaves[0];
        }
        else
        {
            return _currentSpawnWaves[_waveIndex + 1];
        }
    }

    public class WaveRuntimeData
    {
        public float startTime;
        public float finishTime;
        public float currentGold = 0;
        public float bonusGold = 0;
    }

    private void OnValidate()
    {
        if (gameState == null)
        {
            gameState = FindObjectsByType<GameState>(FindObjectsSortMode.None)[0];
        }
    }

    public void DamageTaken(int damage)
    {
        onDamageTaken.Invoke(damage);
    }
}