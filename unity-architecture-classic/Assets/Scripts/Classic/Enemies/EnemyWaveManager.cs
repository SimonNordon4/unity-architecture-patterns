using System;
using Classic.Actors;
using Classic.Game;
using Classic.Items;
using UnityEngine;

namespace Classic.Enemies
{
    [RequireComponent(typeof(EnemyWaveSpawner))]
    public class EnemyWaveManager : ActorComponent
    {
        private EnemyWaveSpawner _enemyWaveSpawner;
        private int _currentWaveIndex = 0;
        [SerializeField] private ChestSpawner chestSpawner;
        [SerializeField] private GameState gameState;
        [SerializeField] private EnemyRoundDefinition roundDefinition;

        private void Awake()
        {
            _enemyWaveSpawner = GetComponent<EnemyWaveSpawner>();            
        }

        private void OnEnable()
        {
            gameState.OnGameStart += StartRoundSpawner;
            _enemyWaveSpawner.OnWaveCompleted += OnWaveCompleted;
            
        }

        private void StartRoundSpawner()
        {
            Reset();
            var currentWaveDefinition = roundDefinition.waves[_currentWaveIndex];
            _enemyWaveSpawner.StartNewWave(currentWaveDefinition);
        }

        private void OnDisable()
        {
            gameState.OnGameStart -= StartRoundSpawner;
            _enemyWaveSpawner.OnWaveCompleted -= OnWaveCompleted;
        }

        private void OnWaveCompleted(Vector3 deathPosition)
        {
            if(_currentWaveIndex + 1 >= roundDefinition.waves.Count)
            {
                Debug.Log("Game won, not spawning chest.");
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