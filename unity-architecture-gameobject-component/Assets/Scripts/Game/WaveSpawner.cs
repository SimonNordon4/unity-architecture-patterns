using System;
using GameObjectComponent.Definitions;
using GameObjectComponent.GameplayComponents;
using GameplayComponents.Life;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameObjectComponent.Game
{
    /// <summary>
    /// Manages the spawning of enemies during a wave.
    /// </summary>
    public class WaveSpawner : GameplayComponent
    {
        public event Action<Vector3> OnWaveCompleted;
        
        [SerializeField] private ActorActionSpawner actionSpawner;

        private bool _waveStarted = false;
        
        private WaveDefinition _currentWaveDefinition;
        private int _spawnIndex = 0;
        private float _waveTime;
        private float[] _actionTimings;

        private bool _bossSpawned = false;
        private int _totalEnemies = 0;
        private int _enemiesKilled = 0;

        private void OnEnemyDeath(Vector3 position)
        {
            _enemiesKilled++;
            Debug.Log($"Enemies killed: {_enemiesKilled}");
            if (_enemiesKilled >= _currentWaveDefinition.TotalEnemyCount())
            {
                OnWaveCompleted?.Invoke(position);
            }
        }

        public void StartNewWave(WaveDefinition waveDefinition)
        {
            Reset();
            _currentWaveDefinition = waveDefinition;
            _totalEnemies = _currentWaveDefinition.TotalEnemyCount();
            GenerateActionTimings();
            _waveStarted = true;
        }

        private void GenerateActionTimings()
        {
            _actionTimings = new float[_totalEnemies];
            for (var i = 0; i < _actionTimings.Length; i++)
            {
                _actionTimings[i] = (float)i / _actionTimings.Length * _currentWaveDefinition.waveDuration;
            }
        }

        private void Update()
        {
            if(!_waveStarted) return;
            if(_bossSpawned) return;
            HandleWaveTime();
            HandleActionSpawn();
            HandleBossSpawn();
        }

        private void HandleWaveTime()
        {

            _waveTime += GameTime.deltaTime;
        }

        private void HandleActionSpawn()
        {
            if(_spawnIndex >= _actionTimings.Length) return;
            if (_waveTime <= _actionTimings[_spawnIndex]) return;
            SpawnAction();
        }

        private void HandleBossSpawn()
        {
            if (_waveTime <= _currentWaveDefinition.waveDuration) return;
            SpawnBossAction();
            _bossSpawned = true;
        }

        private void SubscribeEnemyDeath(Health enemyComponent)
        {
            enemyComponent.OnDeath += () => OnEnemyDeath(enemyComponent.transform.position);
        }

        private void SpawnAction()
        {
            var randomActionIndex = Random.Range(0, _currentWaveDefinition.spawnActions.Count);
            var actionDefinition = _currentWaveDefinition.spawnActions[randomActionIndex];
            
            // Subscribe to the enemies deaths.
            var enemies = actionSpawner.SpawnAction(actionDefinition);
            foreach (var enemy in enemies)
            {
                SubscribeEnemyDeath(enemy.GetComponent<Health>());
            }
            _spawnIndex++;
        }

        private void SpawnBossAction()
        {
            foreach (var action in _currentWaveDefinition.bossActions)
            {
                var enemies = actionSpawner.SpawnAction(action);
                foreach (var enemy in enemies)
                {
                    SubscribeEnemyDeath(enemy.GetComponent<Health>());
                }
            }
        }

        public override void Reset()
        {
            Debug.Log("Resetting wave spawner.");
            _totalEnemies = 0;
            _enemiesKilled = 0;
            _waveTime = 0;
            _spawnIndex = 0;
            _bossSpawned = false;
            _currentWaveDefinition = null;
            _waveStarted = false;
        }

        private void OnDisable()
        {
            OnWaveCompleted = null;
        }
    }
}