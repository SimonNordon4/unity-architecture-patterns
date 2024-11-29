using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{

    public class EnemyHandler : MonoBehaviour
    {
        [SerializeField]
        private ActorPool normalEnemyPool;
        [SerializeField] private Level level;
        private float spawnTimer;
        private const float SPAWN_INTERVAL = 1f;

        private void Start()
        {
            spawnTimer = SPAWN_INTERVAL;
        }

        private void Update()
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = SPAWN_INTERVAL;
            }
        }

        private void SpawnEnemy()
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-level.Bounds.x, level.Bounds.x),
                1,
                Random.Range(-level.Bounds.y, level.Bounds.y)
            );

            var enemy = normalEnemyPool.Get(randomPosition);
        }

    }
}