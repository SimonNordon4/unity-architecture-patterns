using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{

    public class EnemyHandler : MonoBehaviour
    {
        [SerializeField]
        private ActorPool normalEnemyPool;
        [SerializeField] private Level level;
        [SerializeField] private GameState gameState;
        private float spawnTimer;
        private const float SPAWN_INTERVAL = 1f;
        private int spawnedEnemies = 0;
        private int enemiesKilled = 0;

        private void Start()
        {
            spawnTimer = SPAWN_INTERVAL;
        }

        private void OnEnable()
        {
            normalEnemyPool.OnActorReturn.AddListener(EnemyDied);
        }

        private void EnemyDied(PoolableActor actor)
        {
            Debug.Log("Enemy Died!");
            enemiesKilled++;
            if(enemiesKilled == 4)
            {
                gameState.WinGame();
            }
        }

        private void OnDisable()
        {
            normalEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
        }

        private void Update()
        {
            if(spawnedEnemies == 4)
            {
                return;
            }
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = SPAWN_INTERVAL;
            }
        }

        private void SpawnEnemy()
        {
            spawnedEnemies++;
            Vector3 randomPosition = new Vector3(
                Random.Range(-level.Bounds.x, level.Bounds.x),
                0f,
                Random.Range(-level.Bounds.y, level.Bounds.y)
            );

            var enemy = normalEnemyPool.Get(randomPosition);
        }

    }
}