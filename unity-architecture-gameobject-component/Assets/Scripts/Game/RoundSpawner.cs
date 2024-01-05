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
            var currentWaveDefinition = roundDefinition.waves[_currentWaveIndex];
            waveSpawner.StartNewWave(currentWaveDefinition);
        }

        private void OnWaveCompleted(Vector3 deathPosition)
        {
            if(_currentWaveIndex + 1 >= roundDefinition.waves.Count)
            {
                return;
            }
            
            var bossChest = chestSpawner.SpawnChest(ChestType.Medium, deathPosition);
            bossChest.onPickedUp.AddListener(OnBossChestPickedUp);
        }

        private void OnBossChestPickedUp()
        {
            StartNextWave();
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
            Debug.Log("Starting Wave Spawner");
            StartRoundSpawner();
        }
    }
}