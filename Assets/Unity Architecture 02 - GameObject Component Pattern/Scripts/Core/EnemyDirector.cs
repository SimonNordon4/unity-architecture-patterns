using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(EnemySpawner))]
    public class EnemyDirector : MonoBehaviour
    {
        private EnemySpawner _spawner;
        private EnemyDirectorWave[] _waves;
        private int _currentWave = 0;
        
        public int enemyKillProgressCount { get; private set; }
        public int totalEnemiesKilled { get; private set; }

        private float _healthMultiplier;
        private float _damageMultiplier;

        private bool _progressPaused;
        private int _maxEnemiesAlive;
        private float _currentSpawnRate;
        private float _timeSinceLastSpawn;

        private readonly List<PoolableActor> _activeEnemies = new();
        private readonly List<PoolableActor> _activeBosses = new();
        
        private List<EnemyType> _enemyTypes = new() {EnemyType.Normal};

        private void Awake()
        {
            _spawner = GetComponent<EnemySpawner>();
            _waves = GetComponents<EnemyDirectorWave>().OrderBy(x => x.breakPoint).ToArray();
        }

        void OnEnable()
        {
            _spawner.OnEnemyDied.AddListener(EnemyDied);
            _spawner.OnBossDied.AddListener(BossDied);
        }

        void OnDisable()
        {
            _spawner.OnEnemyDied.RemoveListener(EnemyDied);
            _spawner.OnBossDied.RemoveListener(BossDied);
        }

        private void EnemyDied(PoolableActor actor)
        {
            totalEnemiesKilled++;

            if(_progressPaused) return;

            enemyKillProgressCount++;

            // If the enemy kill count has hit the next breakpoint, we want to spawn the bosses of the current wave.
            if (enemyKillProgressCount >= _waves[_currentWave].breakPoint)
            {
                foreach(var type in _waves[_currentWave].bossTypes)
                {
                    SpawnBoss(type);
                }
            }

            // switch (enemyKillProgressCount)
            // {
            //     case 0:
            //         spawnableEnemies = new List<EnemyController>{normalEnemyPrefab};
            //         healthMultiplier = 1f;
            //         damageMultiplier = 1f;
            //         maxEnemiesAlive = 3;
            //         break;
            //     case 10:
            //         spawnableEnemies = new List<EnemyController> {normalEnemyPrefab, fastEnemyPrefab};
            //         healthMultiplier = 1.25f;
            //         damageMultiplier = 1.25f;
            //         maxEnemiesAlive = 5;
            //         break;
            //     case 20:
            //         spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,normalEnemyPrefab, fastEnemyPrefab, bigEnemyPrefab};
            //         healthMultiplier = 1.5f;
            //         damageMultiplier = 1.5f;
            //         maxEnemiesAlive = 7;
            //         break;
            //     case 40:
            //         spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,normalEnemyPrefab, fastEnemyPrefab,fastEnemyPrefab, bigEnemyPrefab};
            //         healthMultiplier = 2f;
            //         damageMultiplier = 2f;
            //         maxEnemiesAlive = 9;
            //         break;
            //     case 51:
            //         spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,rangedEnemyPrefab};
            //         healthMultiplier = 2.25f;
            //         damageMultiplier = 2.25f;
            //         maxEnemiesAlive = 10;
            //         break;
            //     case 71:
            //         spawnableEnemies = new List<EnemyController> {normalEnemyPrefab, chargerEnemyPrefab};
            //         healthMultiplier = 3f;
            //         damageMultiplier = 3f;
            //         maxEnemiesAlive = 12;
            //         break;
            //     case 91:
            //         spawnableEnemies = new List<EnemyController> {fastEnemyPrefab};
            //         healthMultiplier = 2f;
            //         damageMultiplier = 2f;
            //         maxEnemiesAlive = 50;
            //         break;
            //     case 221:
            //         spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, normalEnemyPrefab };
            //         healthMultiplier = 5f;
            //         damageMultiplier = 5f;
            //         maxEnemiesAlive = 15;
            //         break;
            //     case 251:
            //         spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, wandererRangedEnemyPrefab };
            //         healthMultiplier = 6f;
            //         damageMultiplier = 6f;
            //         maxEnemiesAlive = 18;
            //         break;
            //     case 281:
            //         spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, wandererRangedEnemyPrefab, wandererExploderEnemyPrefab };
            //         healthMultiplier = 7f;
            //         damageMultiplier = 7f;
            //         maxEnemiesAlive = 21;
            //         break;
            //     case 311:
            //         spawnableEnemies = new List<EnemyController> { fastEnemyPrefab, normalEnemyPrefab, chargerEnemyPrefab, rangedEnemyPrefab };
            //         healthMultiplier = 8f;
            //         damageMultiplier = 8f;
            //         maxEnemiesAlive = 24;
            //         break;
            //     case 341:
            //         spawnableEnemies = new List<EnemyController> { bigEnemyPrefab, normalEnemyPrefab, bigEnemyPrefab, wandererExploderEnemyPrefab };
            //         healthMultiplier = 9f;
            //         damageMultiplier = 9f;
            //         maxEnemiesAlive = 27;
            //         break;
            //     case 371:
            //         spawnableEnemies = new List<EnemyController> { normalEnemyPrefab, fastEnemyPrefab, bigEnemyPrefab, chargerEnemyPrefab, rangedEnemyPrefab, wandererEnemyPrefab, wandererRangedEnemyPrefab, wandererExploderEnemyPrefab};
            //         healthMultiplier = 10f;
            //         damageMultiplier = 10f;
            //         maxEnemiesAlive = 30;
            //         break;
            // }

        }
        
        
        private void BossDied(PoolableActor bossActor)
        {
            _activeBosses.Remove(bossActor);

            if (_activeBosses.Count == 0)
            {
                ProgressToNextWave();
            }
        }

        private void ProgressToNextWave()
        {
            _currentWave++;
            var newWave = _waves[_currentWave];
            _enemyTypes = newWave.enemyTypes;
            _healthMultiplier = newWave.healthMultiplier;
            _damageMultiplier = newWave.damageMultiplier;
            _maxEnemiesAlive = newWave.maxEnemiesAlive;
        }

        private void Update()
        {
            _currentSpawnRate = _activeEnemies.Count > 0 ? Mathf.Clamp01(_activeEnemies.Count / (float)_maxEnemiesAlive) : 1f; 
            _timeSinceLastSpawn += Time.deltaTime;
            
            if (_timeSinceLastSpawn > _currentSpawnRate && _activeEnemies.Count < _maxEnemiesAlive)
            {
                _timeSinceLastSpawn = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            var enemyType = _enemyTypes[Random.Range(0, _enemyTypes.Count)];
            var enemy = _spawner.SpawnEnemy(enemyType);
            if (enemy.TryGetComponent<Stats>(out var stats))
            {
                stats.MaxHealth.AddModifier(new Modifier
                {
                    statType = StatType.MaxHealth,
                    modifierValue = Mathf.RoundToInt(_healthMultiplier * 100),
                    isFlatPercentage = false
                });
                
                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt(_damageMultiplier * 100),
                    isFlatPercentage = false
                });
            }
            
            _activeEnemies.Add(enemy);
        }

        private void SpawnBoss(EnemyType type)
        {
            var enemy = _spawner.SpawnBoss(type);
            if (enemy.TryGetComponent<Stats>(out var stats))
            {
                stats.MaxHealth.AddModifier(new Modifier
                {
                    statType = StatType.MaxHealth,
                    modifierValue = Mathf.RoundToInt(_healthMultiplier * 100),
                    isFlatPercentage = false
                });
                
                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt(_damageMultiplier * 100),
                    isFlatPercentage = false
                });
            }
            
            _activeBosses.Add(enemy);

            // Stop progressing the enemy kill count.
            _progressPaused = true;
        }
    }
}