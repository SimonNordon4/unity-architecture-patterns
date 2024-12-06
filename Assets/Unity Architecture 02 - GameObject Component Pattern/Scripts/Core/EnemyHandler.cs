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
        public float spawnInterval = 1f;
        private int spawnedEnemies = 0;
        private int enemiesKilled = 0;
        public int maxEnemies = 4;
        public int enemyToKill = 20;

        private void Start()
        {
            spawnTimer = spawnInterval;
        }

        private void OnEnable()
        {
            normalEnemyPool.OnActorReturn.AddListener(EnemyDied);
        }

        private void EnemyDied(PoolableActor actor)
        {
            Debug.Log("Enemy Died!");
            enemiesKilled++;
            if(enemiesKilled == enemyToKill)
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
            if(spawnedEnemies == maxEnemies)
            {
                return;
            }
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
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