using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(EnemySpawner))]
    public class EnemyDirector : MonoBehaviour
    {
        private EnemySpawner _spawner;
        
        public int enemyKillProgressCount { get; private set; }
        public int totalEnemiesKilled { get; private set; }

        private float _healthMultiplier;
        private float _damageMultiplier;

        private int _maxEnemiesAlive;
        private float _currentSpawnRate;
        private float _timeSinceLastSpawn;

        private readonly List<PoolableActor> _activeEnemies = new();
        private readonly List<PoolableActor> _activeBosses = new();
        
        private List<EnemyType> _spawnableEnemies = new() {EnemyType.Normal};

        private void Awake()
        {
            _spawner = GetComponent<EnemySpawner>();
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
            
        }
        
        private void BossDied(PoolableActor arg0)
        {
            
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
            var enemyType = _spawnableEnemies[Random.Range(0, _spawnableEnemies.Count)];
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
    }
}