using GameObjectComponent.Definitions;
using GameObjectComponent.Items;
using GameplayComponents;
using Pools;
using UnityEngine;

namespace GameObjectComponent.Game
{
    [RequireComponent(typeof(WaveSpawner))]
    [RequireComponent(typeof(ActorPool))]
    public class RoundSpawner : GameplayComponent
    {
        private WaveSpawner _waveSpawner;
        private int _currentWaveIndex = 0;
        [SerializeField] private ChestSpawner chestSpawner;
        [SerializeField] private GameState gameState;
        [SerializeField] private RoundDefinition roundDefinition;

        private void Awake()
        {
            _waveSpawner = GetComponent<WaveSpawner>();            
        }

        private void OnEnable()
        {
            _waveSpawner.OnWaveCompleted += OnWaveCompleted;
        }
        private void OnDisable()
        {
            _waveSpawner.OnWaveCompleted -= OnWaveCompleted;
        }

        public void StartRoundSpawner()
        {
            var currentWaveDefinition = roundDefinition.waves[_currentWaveIndex];
            _waveSpawner.StartNewWave(currentWaveDefinition);
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
    }
}