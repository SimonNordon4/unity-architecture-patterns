using System;
using GameObjectComponent.Definitions;
using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using UnityEngine;
using UnityEngine.Events;
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
        public UnityEvent<Vector3> onWaveActorDied = new();
        
        [SerializeField] private ActorActionSpawner actionSpawner;

        private bool _waveStarted = false;
        
        public WaveDefinition currentWaveDefinition { get; private set; }
        private int _spawnIndex = 0;
        private float _waveTime;
        private float[] _actionTimings;

        private bool _waveFinaleActionSpawned = false;

        public int actorsKilledThisWave { get; private set; } = 0;

        public int totalActorsInWave { get; private set; } = 0;

        public void StartNewWave(WaveDefinition waveDefinition)
        {
            Reset();
            currentWaveDefinition = waveDefinition;
            totalActorsInWave = currentWaveDefinition.TotalActorsCount();
            GenerateActionTimings();
            _waveStarted = true;
        }
        
        
        public void Reset()
        {
            totalActorsInWave = 0;
            currentWaveDefinition = null;
            _waveStarted = false;
            _waveFinaleActionSpawned = false;
            _spawnIndex = 0;
            _waveTime = 0;
            actorsKilledThisWave = 0;
            _actionTimings = null;
        }

        private void GenerateActionTimings()
        {
            _actionTimings = new float[currentWaveDefinition.normalEnemies];
            for (var i = 0; i < _actionTimings.Length; i++)
            {
                _actionTimings[i] = (float)i / _actionTimings.Length * currentWaveDefinition.waveDuration;
            }
        }

        private void OnActorDied(DeathHandler actor)
        {
            actorsKilledThisWave++;
            if (actorsKilledThisWave >= totalActorsInWave)
            {
                Debug.Log("Wave Completed, total actors killed is equal to total actors.");
                Debug.Log("Actors Returned: " + actorsKilledThisWave + " Total Actors: " + totalActorsInWave);
                OnWaveCompleted?.Invoke(actor.transform.position);
            }
            onWaveActorDied?.Invoke(actor.transform.position);
            
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
            if(_spawnIndex >= currentWaveDefinition.normalEnemies) return;
            if (_waveTime <= _actionTimings[_spawnIndex]) return;
            SpawnAction();
        }

        private void HandleFinaleActionSpawn()
        {
            if (_waveTime <= currentWaveDefinition.waveDuration) return;
            SpawnFinaleAction();
            _waveFinaleActionSpawned = true;
        }


        private void SpawnAction()
        {
            var randomActionIndex = Random.Range(0, currentWaveDefinition.spawnActions.Count);
            var actionDefinition = currentWaveDefinition.spawnActions[randomActionIndex];
            SpawnActors(actionDefinition);
            // This allows us to skip over each theoretical enemy spawn.
            _spawnIndex += actionDefinition.numberOfEnemiesToSpawn;
        }

        private void SpawnFinaleAction()
        {
            foreach (var action in currentWaveDefinition.bossActions)
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
                if (actor.TryGetComponent<DeathHandler>(out var deathHandler))
                {
                    deathHandler.OnDeath += OnActorDied;    
                }

                if (actor.TryGetComponent<Stats>(out var stats))
                {
                    ApplyWaveHealthModifier(stats);
                    ApplyWaveDamageModifier(stats);
                }
            }
        }
        
        private void ApplyWaveHealthModifier(Stats stats)
        {
            var health = stats.GetStat(StatType.MaxHealth);
            health.Reset();
            
            // apply health modifiers.
            var healthPercentage = Random.Range(currentWaveDefinition.healthMultiplier.x,
                currentWaveDefinition.healthMultiplier.y);

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
            var damagePercentage = Random.Range(currentWaveDefinition.damageMultiplier.x,
                currentWaveDefinition.damageMultiplier.y);

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