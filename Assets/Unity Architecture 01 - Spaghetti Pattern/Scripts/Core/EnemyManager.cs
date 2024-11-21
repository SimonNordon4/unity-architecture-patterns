using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityArchitecture.SpaghettiPattern
{
    public class EnemyManager : MonoBehaviour
    {
        
        private static EnemyManager _instance;

        public static EnemyManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<EnemyManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        
        [Header("References")]
        public GameManager gameManager;
        public ChestManager chestManager;
        public Transform playerTarget;
        
        [Header("Enemy Stats")]
        public float healthMultiplier = 1f;
        public float damageMultiplier = 1f;

        [Header("Spawn Settings")]
        public int maxEnemiesAlive = 3;
        public float baseSpawnRate = 1f;
        public float currentSpawnRate = 1f;
        private float _timeSinceLastSpawn = 0f;

        // Tracking active enemies and bosses
        public int enemyKillProgressCount = 0;
        public bool progressPaused = false;
        public int totalEnemiesKilled = 0;

        // Lists to keep track of active enemies and bosses
        public List<EnemyController> activeEnemies = new();
        public List<EnemyController> activeBosses = new();
        public List<EnemyController> spawnableEnemies = new();
        
        [Header("Prefabs")]
        public EnemyController normalEnemyPrefab;
        public EnemyController fastEnemyPrefab;
        public EnemyController bigEnemyPrefab;
        public EnemyController chargerEnemyPrefab;
        public EnemyController rangedEnemyPrefab;
        public EnemyController wandererEnemyPrefab;
        public EnemyController wandererRangedEnemyPrefab;
        public EnemyController wandererExploderEnemyPrefab;
        
        public EnemyController normalBossEnemyPrefab;
        public EnemyController fastBossEnemyPrefab;
        public EnemyController bigBossEnemyPrefab;
        public EnemyController chargerBossEnemyPrefab;
        public EnemyController rangedBossEnemyPrefab;
        public EnemyController wandererBossEnemyPrefab;
        public EnemyController wandererRangedBossEnemyPrefab;
        public EnemyController wandererExploderBossEnemyPrefab;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!gameManager.isGameActive) return;

            // Increment the spawn timer
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn > GetSpawnRate())
            {
                _timeSinceLastSpawn = 0f;
                // pick a random enemy from the spawnable list
                var enemyPrefab = spawnableEnemies[Random.Range(0, spawnableEnemies.Count)];
                SpawnEnemy(enemyPrefab);
            }
        }

        /// <summary>
        /// Spawns a regular enemy based on the current block's configuration.
        /// </summary>
        private void SpawnEnemy(EnemyController enemyPrefab)
        {

            var enemy = Instantiate(enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            enemy.playerTarget = playerTarget;
            enemy.enemyManager = this;
            enemy.ApplyMultipliers(healthMultiplier, damageMultiplier);
            activeEnemies.Add(enemy);
        }

        /// <summary>
        /// Called when a regular enemy dies.
        /// </summary>
        /// <param name="enemy">The enemy that died.</param>
        public void EnemyDied(EnemyController enemy)
        {
            totalEnemiesKilled++;
            
            if(!progressPaused)
                enemyKillProgressCount++;
            
            chestManager.ReduceChestSpawnTime();
            
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
        }

        /// <summary>
        /// Spawns all boss enemies defined in the current block.
        /// </summary>
        public void SpawnBoss(EnemyController bossEnemyPrefab)
        {
            var enemy = Instantiate(bossEnemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            enemy.playerTarget = playerTarget;
            enemy.enemyManager = this;
            enemy.ApplyMultipliers(healthMultiplier, damageMultiplier);
            activeBosses.Add(enemy);
        }

        /// <summary>
        /// Called when a boss dies.
        /// </summary>
        /// <param name="boss">The boss that died.</param>
        public void BossDied(EnemyController boss)
        {
            if (activeBosses.Contains(boss))
            {
                activeBosses.Remove(boss);
            }
        }

        /// <summary>
        /// Generates a random spawn point around the player within a specified radius.
        /// </summary>
        /// <returns>A random Vector3 position.</returns>
        private Vector3 GetRandomSpawnPoint()
        {
            float spawnRadius = 20f; // Adjust as needed
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 spawnPos = playerTarget.position + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnRadius;
            return spawnPos;
        }

        /// <summary>
        /// Calculates the current spawn rate based on the block's maximum enemies alive.
        /// </summary>
        /// <returns>Spawn interval in seconds.</returns>
        private float GetSpawnRate()
        {
            float currentMaxEnemies = maxEnemiesAlive;
            float remainingEnemyCapacity = currentMaxEnemies > 0 ? 1f - ((float)enemiesAlive / currentMaxEnemies) : 1f;
            remainingEnemyCapacity = Mathf.Clamp01(remainingEnemyCapacity);
            return baseSpawnRate * remainingEnemyCapacity;
        }

        public void Reset()
        {
            Console.Log("EnemyManager.Reset()", LogFilter.Enemy, this); 
            // Destroy all active enemies
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            activeEnemies.Clear();

            // Destroy all active bosses
            foreach (var boss in activeBosses)
            {
                if (boss != null)
                {
                    Destroy(boss.gameObject);
                }
            }
            activeBosses.Clear();

            // Reset tracking variables
            enemiesAlive = 0;
            bossesAlive = 0;
            _timeSinceLastSpawn = 0f;
        }
    }
}
