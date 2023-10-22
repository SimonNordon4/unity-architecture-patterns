using System;
using Classic.Actors;
using Classic.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Classic.Enemies
{
    /// <summary>
    /// Manages the spawning of enemies during a wave.
    /// </summary>
    public class EnemyWaveSpawner : ActorComponent
    {
        public event Action OnWaveCompleted;
        
        [SerializeField] private EnemyActionSpawner actionSpawner;

        private WaveDefinition _currentWaveDefinition;
        private int _spawnIndex = 0;
        private float _waveTime;
        private float[] _actionTimings;

        private bool _bossSpawned = false;
        private int _totalEnemies = 0;
        private int _enemiesKilled = 0;

        private void OnEnemyDeath(  )
        {
            _enemiesKilled++;
            if (_enemiesKilled >= _currentWaveDefinition.TotalEnemyCount())
            {
                OnWaveCompleted?.Invoke();
            }
        }

        public void StartNewWave(WaveDefinition waveDefinition)
        {
            Reset();
            _currentWaveDefinition = waveDefinition;
            _totalEnemies = _currentWaveDefinition.TotalEnemyCount();
            GenerateActionTimings();
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
            HandleWaveTime();
            HandleActionSpawn();
            HandleBossSpawn();
        }

        private void HandleWaveTime()
        {
            if (_bossSpawned) return;
            _waveTime += GameTime.deltaTime;
        }

        private void HandleActionSpawn()
        {
            if (_bossSpawned || _waveTime <= _actionTimings[_spawnIndex] || _spawnIndex >= _actionTimings.Length) return;
            SpawnAction();
        }

        private void HandleBossSpawn()
        {
            if (_waveTime <= _currentWaveDefinition.waveDuration) return;
            SpawnBossAction();
            _bossSpawned = true;
        }

        private void SubscribeEnemyDeath(ActorHealth enemyComponent)
        {
            enemyComponent.OnDeath += () => OnEnemyDeath( );
        }

        private void SpawnAction()
        {
            var randomActionIndex = Random.Range(0, _currentWaveDefinition.spawnActions.Count);
            var actionDefinition = _currentWaveDefinition.spawnActions[randomActionIndex];
            
            // Subscribe to the enemies deaths.
            var enemies = actionSpawner.SpawnAction(actionDefinition);
            foreach (var enemy in enemies)
            {
                SubscribeEnemyDeath(enemy.GetComponent<ActorHealth>());
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
                    SubscribeEnemyDeath(enemy.GetComponent<ActorHealth>());
                }
            }
        }

        public override void Reset()
        {
            _totalEnemies = 0;
            _enemiesKilled = 0;
            _waveTime = 0;
            _spawnIndex = 0;
            _bossSpawned = false;
            _currentWaveDefinition = null;
        }

        private void OnDisable()
        {
            OnWaveCompleted = null;
        }
    }
}