using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.Debugging
{
    public class RoundSpawnerDebugger : DebugComponent
    {
        public GameState gameState;
        public WaveSpawner waveSpawner;
        public WaveDefinition waveDefinition;
        public bool spawnWaveOnGameStart = false;

        private void OnEnable()
        {
            gameState.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            if (spawnWaveOnGameStart)
            {
                waveSpawner.StartNewWave(waveDefinition);
            }
        }

        private void OnDisable()
        {
            gameState.OnGameStart -= OnGameStart;
        }
    }
}