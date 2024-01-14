using GameObjectComponent.Definitions;
using GameObjectComponent.Items;
using GameplayComponents;
using UnityEngine;

namespace GameObjectComponent.Game
{
    public class RoundSpawner : GameplayComponent
    {
        [SerializeField]private WaveSpawner waveSpawner;
        private int _currentWaveIndex = 0;
        private WaveDefinition _currentWaveDefinition;
        [SerializeField] private ChestSpawner chestSpawner;
        [SerializeField] private GameState gameState;
        [SerializeField] private RoundDefinition roundDefinition;

        public int roundsCompleted => _currentWaveIndex;
        public int actorKilledThisRound { get; private set; }
        public int totalActorsInRound { get; private set; }

        private void OnEnable()
        {
            waveSpawner.OnWaveCompleted += OnWaveCompleted;
            waveSpawner.onWaveActorDied.AddListener(OnWaveActorDied);
            
        }

        private void OnWaveActorDied(Vector3 arg0)
        {
            actorKilledThisRound++;
        }

        private void OnDisable()
        {
            waveSpawner.OnWaveCompleted -= OnWaveCompleted;
            waveSpawner.onWaveActorDied.RemoveListener(OnWaveActorDied);
        }

        public void StartRoundSpawner()
        {
            if (roundDefinition == null) return;
            _currentWaveDefinition = roundDefinition.waves[_currentWaveIndex];
            waveSpawner.StartNewWave(_currentWaveDefinition);
            totalActorsInRound = _currentWaveDefinition.TotalActorsCount();
            actorKilledThisRound = 0;
        }

        private void OnWaveCompleted(Vector3 deathPosition)
        {
            if(_currentWaveIndex + 1 >= roundDefinition.waves.Count)
            {
                FinalWaveCompleted();
                return;
            }
            var bossChest = chestSpawner.SpawnChest(_currentWaveDefinition.rewardChest, deathPosition);
            bossChest.onPickedUp.AddListener(OnBossChestPickedUp);
        }

        private void OnBossChestPickedUp()
        {
            StartNextWave();
        }
        
        private void FinalWaveCompleted()
        {
            Debug.Log("Final Wave Completed");
            gameState.WinGame();
        }
        
        private void StartNextWave()
        {
            _currentWaveIndex++;
            if (_currentWaveIndex >= roundDefinition.waves.Count)
            {
                gameState.WinGame();
                return;
            }
            StartRoundSpawner();
        }

        public override void OnGameStart()
        {
            _currentWaveIndex = 0;
            StartRoundSpawner();
        }
    }
}