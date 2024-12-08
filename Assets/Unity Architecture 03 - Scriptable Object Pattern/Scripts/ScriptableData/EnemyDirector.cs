using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class EnemyDirector : ScriptableData
    {
        public UnityEvent allWavesFinished = new();

        [SerializeField] private EnemiesPool enemiesPool;
        [SerializeField] private EnemyDiedEvent enemyDiedEvent;
        [SerializeField] private EnemyDiedEvent bossDiedEvent;
        [SerializeField] private EnemyDirectorWave[] waves;
        private int _waveIndex = 0;
        private bool _progressPaused;
        
        private readonly List<GameObject> _activeEnemies = new();
        private readonly List<GameObject> _activeBosses = new();
        
        public int EnemyKillProgressCount { get; private set; }
        public int TotalEnemiesKilled { get; private set; }
        public bool IsFightingBoss => _progressPaused;
        public int EnemiesToKill => waves[_waveIndex].enemiesToKill;
        public int MaxEnemiesAlive => waves[_waveIndex].maxEnemiesAlive;
        public int EnemiesLeft => _activeEnemies.Count;
        public int BossesLeft => _activeBosses.Count;
        public int TotalEnemiesToKill => 400;
        public float ProgressPercentage => (float)EnemyKillProgressCount / EnemiesToKill;

        public override void ResetData()
        {
            Debug.Log("Resetting data");
            _waveIndex = 0;
            _progressPaused = false;
            EnemyKillProgressCount = 0;
            TotalEnemiesKilled = 0;
            _activeEnemies.Clear();
            _activeBosses.Clear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            enemyDiedEvent.OnEnemyDied.AddListener(EnemyDied);
            bossDiedEvent.OnEnemyDied.AddListener(BossDied);
        }

        protected override void OnDisable()
        {
            enemyDiedEvent.OnEnemyDied.RemoveListener(EnemyDied);
            bossDiedEvent.OnEnemyDied.RemoveListener(BossDied);
            base.OnDisable();
        }

        
        private void EnemyDied(GameObject actor)
        {
            _activeEnemies.Remove(actor);
            TotalEnemiesKilled++;

            if (_progressPaused) return;

            EnemyKillProgressCount++;

            // If the enemy kill count has hit the next breakpoint, we want to spawn the bosses of the current wave & pause progress.
            if (EnemyKillProgressCount >= waves[_waveIndex].enemiesToKill && !_progressPaused)
            {
                // If there are no bosses in the current wave, we want to progress to the next wave.
                if (waves[_waveIndex].bossTypes.Count == 0)
                {
                    ProgressToNextWave();
                    return;
                }

                foreach (var type in waves[_waveIndex].bossTypes)
                {
                    SpawnBoss(type);
                    _progressPaused = true;
                }
            }
        }
        
        private void BossDied(GameObject bossActor)
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

            if (_waveIndex >= waves.Length)
            {
                allWavesFinished?.Invoke();
                _progressPaused = true;
            }
        }
        
        public void SpawnEnemy()
        {
            // If progress is paused, we only want to spawn enemies that are meant to be spawned alongside the boss.
            List<EnemyType> enemySpawnPool;

            if (waves[_waveIndex].bossEnemyTypes.Count <=
                0) // If there are no bosses this waves, spawn normal enemies always.
            {
                enemySpawnPool = waves[_waveIndex].enemyTypes;
            }
            else if (_progressPaused) // If Progress is paused and there are bosses this wave, only spawn boss enemies.
            {
                enemySpawnPool = waves[_waveIndex].bossEnemyTypes;
            }
            else // Normally, just spawn normal enemies.
            {
                enemySpawnPool = waves[_waveIndex].enemyTypes;
            }

            var enemyType = waves[_waveIndex].enemyTypes[Random.Range(0, enemySpawnPool.Count)];
            var enemy = enemiesPool.GetEnemyByType(enemyType);
            if (enemy.TryGetComponent<Stats>(out var stats))
            {
                stats.MaxHealth.AddModifier(new Modifier
                {
                    statType = StatType.MaxHealth,
                    modifierValue = Mathf.RoundToInt((waves[_waveIndex].healthMultiplier - 1) * 100),
                    isFlatPercentage = false
                });

                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt((waves[_waveIndex].damageMultiplier - 1) * 100),
                    isFlatPercentage = false
                });
            }

            _activeEnemies.Add(enemy);
        }
        
        private void SpawnBoss(EnemyType type)
        {
            var enemy = enemiesPool.GetBossByType(type);
            if (enemy.TryGetComponent<Stats>(out var stats))
            {
                stats.MaxHealth.AddModifier(new Modifier
                {
                    statType = StatType.MaxHealth,
                    modifierValue = Mathf.RoundToInt(waves[_waveIndex].healthMultiplier * 100),
                    isFlatPercentage = false
                });

                stats.Damage.AddModifier(new Modifier
                {
                    statType = StatType.Damage,
                    modifierValue = Mathf.RoundToInt(waves[_waveIndex].damageMultiplier * 100),
                    isFlatPercentage = false
                });
            }

            _activeBosses.Add(enemy);
        }
    }
}