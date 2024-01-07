using GameObjectComponent.Definitions;
using GameObjectComponent.Items;
using GameplayComponents;
using Pools;
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

        private void OnEnable()
        {
            waveSpawner.OnWaveCompleted += OnWaveCompleted;
        }
        private void OnDisable()
        {
            waveSpawner.OnWaveCompleted -= OnWaveCompleted;
        }

        public void StartRoundSpawner()
        {
            Debug.Log($"Starting Round Spawner on wave {roundDefinition.waves[_currentWaveIndex].name}");
            _currentWaveDefinition = roundDefinition.waves[_currentWaveIndex];
            waveSpawner.StartNewWave(_currentWaveDefinition);
        }

        private void OnWaveCompleted(Vector3 deathPosition)
        {
            Debug.Log("Wave Completed");
            if(_currentWaveIndex + 1 >= roundDefinition.waves.Count)
            {
                FinalWaveCompleted();
                return;
            }
            
            Debug.Log("Spawning boss chest at death position");
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