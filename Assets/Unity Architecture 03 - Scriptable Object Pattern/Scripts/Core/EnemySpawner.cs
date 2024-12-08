using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]private EnemyDirector director;
        
        private float _currentSpawnRate;
        private float _timeSinceLastSpawn;


        private void Update()
        {
            _currentSpawnRate = director.EnemiesLeft > 0
                ? Mathf.Clamp01(director.EnemiesLeft / (float)director.MaxEnemiesAlive)
                : 1f;
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn > _currentSpawnRate && director.EnemiesLeft < director.MaxEnemiesAlive)
            {
                _timeSinceLastSpawn = 0f;
                director.SpawnEnemy();
            }
        }
    }
}