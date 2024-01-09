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
        
        public void StartNewWave(WaveDefinition waveDefinition)
        {
            Reset();
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

        private void OnActorDied(DeathHandler actor)
        {
            _actorsReturned++;
            Debug.Log("Actor returned: Total actors returned = " + _actorsReturned + " Total actors = " + _totalActors);
            if (_actorsReturned >= _totalActors)
            {
                OnWaveCompleted?.Invoke(actor.transform.position);
            }
            
            actor.OnDeath -= OnActorDied;
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
            if(_spawnIndex >= _currentWaveDefinition.normalEnemies) return;
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
            SpawnActors(actionDefinition);
            _spawnIndex++;
        }

        private void SpawnFinaleAction()
        {
            foreach (var action in _currentWaveDefinition.bossActions)
            {
                SpawnActors(action);
            }
        }

        private void SpawnActors(SpawnActionDefinition actionDefinition)
        {
            // Subscribe to the enemies deaths.
            var actors = actionSpawner.SpawnAction(actionDefinition);

            foreach (var actor in actors)
            {
                if (actor.TryGetComponentDeep<DeathHandler>(out var deathHandler) == false)
                {
                    deathHandler.OnDeath += OnActorDied;    
                }

                if (!actor.TryGetComponent<Stats>(out var stats)) continue;
                ApplyWaveHealthModifier(stats);
                ApplyWaveDamageModifier(stats);
            }
        }
        
        private void ApplyWaveHealthModifier(Stats stats)
        {
            var health = stats.GetStat(StatType.MaxHealth);
            health.Reset();
            
            // apply health modifiers.
            var healthPercentage = Random.Range(_currentWaveDefinition.healthMultiplier.x,
                _currentWaveDefinition.healthMultiplier.y);

            var healthMod = new Modifier
            {
                modifierType = ModifierType.Percentage,
                modifierValue = healthPercentage
            };
                    
            health.AddModifier(healthMod);
        }

        private void ApplyWaveDamageModifier(Stats stats)
        {
            var rangedDamage = stats.GetStat(StatType.RangedDamage);
            var meleeDamage = stats.GetStat(StatType.MeleeDamage);
            
            rangedDamage.Reset();
            meleeDamage.Reset();
            // apply health modifiers.
            var damagePercentage = Random.Range(_currentWaveDefinition.healthMultiplier.x,
                _currentWaveDefinition.healthMultiplier.y);

            var damageMod = new Modifier
            {
                modifierType = ModifierType.Percentage,
                modifierValue = damagePercentage
            };
                    
            rangedDamage.AddModifier(damageMod);
            meleeDamage.AddModifier(damageMod);
        }

        public override void OnGameEnd()
        {
            Reset();
        }
    }
}