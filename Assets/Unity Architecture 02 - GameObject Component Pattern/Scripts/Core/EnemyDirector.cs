using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(EnemySpawner))]
    [RequireComponent(typeof(EnemyDirectorWave))]
    public class EnemyDirector : MonoBehaviour
    {
        private static EnemyDirector _instance;
        public static EnemyDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<EnemyDirector>();
                }

                return _instance;
            }
        }

        public UnityEvent allWavesFinished = new();

        private EnemySpawner _spawner;
        private EnemyDirectorWave[] _waves;
        private int _waveIndex = 0;

        public int EnemyKillProgressCount { get; private set; }
        public int TotalEnemiesKilled { get; private set; }
        public bool FightingBoss => _progressPaused;
        public int CurrentWave => _waveIndex;
        public int EnemiesToKill => 400;
        public int BossesToDefeat => _activeBosses.Count;

        private bool _progressPaused;
        private float _currentSpawnRate;
        private float _timeSinceLastSpawn;

        private readonly List<PoolableActor> _activeEnemies = new();
        private readonly List<PoolableActor> _activeBosses = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _spawner = GetComponent<EnemySpawner>();
            _waves = GetComponents<EnemyDirectorWave>().OrderBy(x => x.enemiesToKill).ToArray();
        }

        void OnEnable()
        {
            _spawner.OnEnemyDied.AddListener(EnemyDied);
            _spawner.OnBossDied.AddListener(BossDied);
            _progressPaused = false;
        }

        void OnDisable()
        {
            _spawner.OnEnemyDied.RemoveListener(EnemyDied);
            _spawner.OnBossDied.RemoveListener(BossDied);
        }

        private void EnemyDied(PoolableActor actor)
        {
            _activeEnemies.Remove(actor);
            TotalEnemiesKilled++;

            if (_progressPaused) return;

            EnemyKillProgressCount++;

            // If the enemy kill count has hit the next breakpoint, we want to spawn the bosses of the current wave & pause progress.
            if (EnemyKillProgressCount >= _waves[_waveIndex].enemiesToKill && !_progressPaused)
            {
                // If there are no bosses in the current wave, we want to progress to the next wave.
                if (_waves[_waveIndex].bossTypes.Count == 0)
                {
                    ProgressToNextWave();
                    return;
                }

                foreach (var type in _waves[_waveIndex].bossTypes)
                {
                    SpawnBoss(type);
                    _progressPaused = true;
                }
            }
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
            _waveIndex++;

            if (_waveIndex >= _waves.Length)
            {
                allWavesFinished?.Invoke();
                _progressPaused = true;
            }
        }

        private void Update()
        {
            _currentSpawnRate = _activeEnemies.Count > 0
                ? Mathf.Clamp01(_activeEnemies.Count / (float)_waves[_waveIndex].maxEnemiesAlive)
                : 1f;
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn > _currentSpawnRate && _activeEnemies.Count < _waves[_waveIndex].maxEnemiesAlive)
            {
                _timeSinceLastSpawn = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            // If progress is paused, we only want to spawn enemies that are meant to be spawned alongside the boss.
            List<EnemyType> enemySpawnPool;

            if (_waves[_waveIndex].bossEnemyTypes.Count <=
                0) // If there are no bosses this waves, spawn normal enemies always.
            {
                enemySpawnPool = _waves[_waveIndex].enemyTypes;
            }
            else if (_progressPaused) // If Progress is paused and there are bosses this wave, only spawn boss enemies.
            {
                enemySpawnPool = _waves[_waveIndex].bossEnemyTypes;
            }
            else // Normally, just spawn normal enemies.
            {
                enemySpawnPool = _waves[_waveIndex].enemyTypes;
            }

            var enemyType = _waves[_waveIndex].enemyTypes[Random.Range(0, enemySpawnPool.Count)];
            var enemy = _spawner.SpawnEnemy(enemyType);
            if (enemy.TryGetComponent<Stats>(out var stats))
            {
                stats.MaxHealth.AddModifier(new Modifier
                {
                    statType = StatType.MaxHealth,
                    modifierValue = Mathf.RoundToInt((_waves[_waveIndex].healthMultiplier - 1) * 100),
                    isFlatPercentage = false
                });

                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt((_waves[_waveIndex].damageMultiplier - 1) * 100),
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
                    modifierValue = Mathf.RoundToInt(_waves[_waveIndex].healthMultiplier * 100),
                    isFlatPercentage = false
                });

                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt(_waves[_waveIndex].damageMultiplier * 100),
                    isFlatPercentage = false
                });
            }

            _activeBosses.Add(enemy);
        }
    }
}