using System;
using GameObjectComponent.Definitions;
using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameObjectComponent.Game
{
    /// <summary>
    /// Manages the spawning of enemies during a wave.
    /// <remarks> This may be the most convoluted class as the managing of waves is tricky.
    /// I've decided to track when Actors are returned to the pool as counting as being dead,
    /// seeing as they can be cancelled before spawning in.</remarks>
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

        private bool _waveFinaleActionSpawned = false;
        private int _totalActors = 0;
        private int _actorsReturned = 0;
        
        private void OnEnable()
        {
            actionSpawner.pool.OnActorReturn += OnActorReturned;
        }
        
        private void OnDisable()
        {
            actionSpawner.pool.OnActorReturn -= OnActorReturned;
        }

        public void StartNewWave(WaveDefinition waveDefinition)
        {
            _currentWaveDefinition = waveDefinition;
            _totalActors = _currentWaveDefinition.TotalActorsCount();
            GenerateActionTimings();
            _waveStarted = true;
        }
        
        public void Reset()
        {
            _totalActors = 0;
            _currentWaveDefinition = null;
            _waveStarted = false;
            _waveFinaleActionSpawned = false;
            _spawnIndex = 0;
            _waveTime = 0;
            _actorsReturned = 0;
            _actionTimings = null;
        }

        private void GenerateActionTimings()
        {
            _actionTimings = new float[_totalActors];
            for (var i = 0; i < _actionTimings.Length; i++)
            {
                _actionTimings[i] = (float)i / _actionTimings.Length * _currentWaveDefinition.waveDuration;
            }
        }

        private void OnActorReturned(PoolableActor actor)
        {
            _actorsReturned++;
            if (_actorsReturned >= _totalActors)
            {
                OnWaveCompleted?.Invoke(actor.transform.position);
            }
        }

        private void Update()
        {
            if(!_waveStarted) return;
            if(_waveFinaleActionSpawned) return;
            HandleWaveTime();
            HandleActionSpawn();
            HandleFinaleActionSpawn();
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

        private void HandleFinaleActionSpawn()
        {
            if (_waveTime <= _currentWaveDefinition.waveDuration) return;
            SpawnFinaleAction();
            _waveFinaleActionSpawned = true;
        }


        private void SpawnAction()
        {
            var randomActionIndex = Random.Range(0, _currentWaveDefinition.spawnActions.Count);
            var actionDefinition = _currentWaveDefinition.spawnActions[randomActionIndex];
            
            // Subscribe to the enemies deaths.
            var enemies = actionSpawner.SpawnAction(actionDefinition);
            foreach (var enemy in enemies)
            {
                //SubscribeEnemyDeath(enemy.GetComponent<Health>());
            }
            _spawnIndex++;
        }

        private void SpawnFinaleAction()
        {
            foreach (var action in _currentWaveDefinition.bossActions)
            {
                var enemies = actionSpawner.SpawnAction(action);
            }
        }

        public override void OnGameEnd()
        {
            Reset();
        }
    }
}