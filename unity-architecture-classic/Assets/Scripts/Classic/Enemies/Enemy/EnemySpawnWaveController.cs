using System.Collections.Generic;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemySpawnWaveController : MonoBehaviour
    {
        private List<EnemySpawnWave> _currentSpawnWaves = new();
        private EnemySpawnWave _currentWave;
        
        public RoundTimer timer;
        public Level level;
        public GameState gameState;
        
        public EnemySpawnPhase currentPhase = EnemySpawnPhase.Normal;
        private float _elapsedWaveTime;
        private int _currentWaveSpawnedEnemies;
        private int _currentWaveAliveEnemies;
        private float[] _spawnTimings;

        private void InitializeNewWave()
        {
            _elapsedWaveTime = 0f;
            _currentWaveSpawnedEnemies = 0;
            _currentWaveAliveEnemies = 0;
            currentPhase = EnemySpawnPhase.Normal;

            // Get the spawn timing of each enemy evaluated against the animation curve
            _spawnTimings = new float[_currentWave.totalEnemies];
            for (var i = 0; i < _currentWave.totalEnemies; i++)
            {
                _spawnTimings[i] = _currentWave.spawnRateCurve.Evaluate((float)i / _currentWave.totalEnemies) *
                                   _currentWave.blockTime;
            }
        }
    }
}