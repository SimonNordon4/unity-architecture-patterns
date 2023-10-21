using Classic.Actors;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyWaveSpawner : ActorComponent
    {
        [SerializeField] private EnemyActionSpawner actionSpawner;

        private WaveDefinition _currentWaveDefinition;
        private int _spawnIndex = 0;
        private float _waveTime;
        private float[] _actionTimings;
        private WavePhase _currentPhase = WavePhase.NormalSpawning;

        public void StartNewWave(WaveDefinition waveDefinition)
        {
            _waveTime = 0;
            GenerateActionTimings(waveDefinition);
        }

        private void GenerateActionTimings(WaveDefinition waveDefinition)
        {
            _actionTimings = new float[waveDefinition.TotalEnemyCount()];
            for (var i = 0; i < _actionTimings.Length; i++)
            {
                // TODO: Make this spawn-rate adjustable (like accelerated)
                _actionTimings[i] = (float)i / _actionTimings.Length * waveDefinition.waveDuration;
            }
        }

        private void Update()
        {
            switch (_currentPhase)
            {
                case WavePhase.NormalSpawning:
                    UpdateNormalSpawning();
                    break;  
            }
        }

        private void UpdateNormalSpawning()
        {
            _waveTime += GameTime.deltaTime;

            if (_waveTime > _currentWaveDefinition.waveDuration)
            {
                _currentPhase = WavePhase.BossSpawning;
                return;
            }
                
            if(_waveTime > _actionTimings[_spawnIndex])
            {
                var randomAction = Random.Range(0, _currentWaveDefinition.spawnActions.Count);
                actionSpawner.SpawnAction(_currentWaveDefinition.spawnActions[randomAction]);
                _spawnIndex++;
            }
            
        }
    }

    public enum WavePhase
    {
        NormalSpawning,
        BossSpawning,
        WaitingForWaveEnd,
        WaitingForChestPickup
    }
}