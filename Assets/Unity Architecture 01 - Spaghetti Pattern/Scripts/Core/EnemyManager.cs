using System.Collections.Generic;
using UnityEngine;

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

            currentSpawnRate = activeEnemies.Count > 0 ? Mathf.Clamp01(baseSpawnRate * (activeEnemies.Count / (float)maxEnemiesAlive)) : 1f;            
            // Increment the spawn timer
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn > currentSpawnRate && activeEnemies.Count < maxEnemiesAlive)
            {
                _timeSinceLastSpawn = 0f;
                // pick a random enemy from the spawnable list
                var enemyPrefab = spawnableEnemies[Random.Range(0, spawnableEnemies.Count)];
                SpawnEnemy(enemyPrefab);
            }

            switch (enemyKillProgressCount)
            {
                case 0:
                    spawnableEnemies = new List<EnemyController>{normalEnemyPrefab};
                    healthMultiplier = 1f;
                    damageMultiplier = 1f;
                    maxEnemiesAlive = 3;
                    break;
                case 10:
                    spawnableEnemies = new List<EnemyController> {normalEnemyPrefab, fastEnemyPrefab};
                    healthMultiplier = 1.25f;
                    damageMultiplier = 1.25f;
                    maxEnemiesAlive = 5;
                    break;
                case 20:
                    spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,normalEnemyPrefab, fastEnemyPrefab, bigEnemyPrefab};
                    healthMultiplier = 1.5f;
                    damageMultiplier = 1.5f;
                    maxEnemiesAlive = 7;
                    break;
                case 40:
                    spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,normalEnemyPrefab, fastEnemyPrefab,fastEnemyPrefab, bigEnemyPrefab};
                    healthMultiplier = 2f;
                    damageMultiplier = 2f;
                    maxEnemiesAlive = 9;
                    break;
                case 51:
                    spawnableEnemies = new List<EnemyController> {normalEnemyPrefab,rangedEnemyPrefab};
                    healthMultiplier = 2.25f;
                    damageMultiplier = 2.25f;
                    maxEnemiesAlive = 10;
                    break;
                case 71:
                    spawnableEnemies = new List<EnemyController> {normalEnemyPrefab, chargerEnemyPrefab};
                    healthMultiplier = 3f;
                    damageMultiplier = 3f;
                    maxEnemiesAlive = 12;
                    break;
                case 91:
                    spawnableEnemies = new List<EnemyController> {fastEnemyPrefab, fastEnemyPrefab, fastEnemyPrefab, bigEnemyPrefab};
                    healthMultiplier = 4f;
                    damageMultiplier = 4f;
                    maxEnemiesAlive = 30;
                    break;
                case 121:
                    spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, normalEnemyPrefab };
                    healthMultiplier = 5f;
                    damageMultiplier = 5f;
                    maxEnemiesAlive = 15;
                    break;
                case 151:
                    spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, wandererRangedEnemyPrefab };
                    healthMultiplier = 6f;
                    damageMultiplier = 6f;
                    maxEnemiesAlive = 18;
                    break;
                case 181:
                    spawnableEnemies = new List<EnemyController> { wandererEnemyPrefab, wandererRangedEnemyPrefab, wandererExploderEnemyPrefab };
                    healthMultiplier = 7f;
                    damageMultiplier = 7f;
                    maxEnemiesAlive = 21;
                    break;
                case 211:
                    spawnableEnemies = new List<EnemyController> { fastEnemyPrefab, normalEnemyPrefab, chargerEnemyPrefab, rangedEnemyPrefab };
                    healthMultiplier = 8f;
                    damageMultiplier = 8f;
                    maxEnemiesAlive = 24;
                    break;
                case 241:
                    spawnableEnemies = new List<EnemyController> { bigEnemyPrefab, normalEnemyPrefab, bigEnemyPrefab, wandererExploderEnemyPrefab };
                    healthMultiplier = 9f;
                    damageMultiplier = 9f;
                    maxEnemiesAlive = 27;
                    break;
                case 271:
                    spawnableEnemies = new List<EnemyController> { normalEnemyPrefab, fastEnemyPrefab, bigEnemyPrefab, chargerEnemyPrefab, rangedEnemyPrefab, wandererEnemyPrefab, wandererRangedEnemyPrefab, wandererExploderEnemyPrefab};
                    healthMultiplier = 10f;
                    damageMultiplier = 10f;
                    maxEnemiesAlive = 30;
                    break;
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
            enemy.isBoss = false;
            enemy.countsTowardsProgress = !progressPaused;
            activeEnemies.Add(enemy);
        }

        /// <summary>
        /// Called when a regular enemy dies.
        /// </summary>
        /// <param name="enemy">The enemy that died.</param>
        public void EnemyDied(EnemyController enemy)
        {
            totalEnemiesKilled++;

            if (!progressPaused)
            {
                if(enemy.countsTowardsProgress)
                    enemyKillProgressCount++;

                switch (enemyKillProgressCount)
                {
                    case 30:
                        SpawnBoss(normalBossEnemyPrefab);
                        break;
                    case 50:
                        SpawnBoss(fastBossEnemyPrefab);
                        break;
                    case 70:
                        SpawnBoss(rangedBossEnemyPrefab);
                        break;
                    case 90:
                        SpawnBoss(chargerEnemyPrefab);
                        break;
                    case 120:
                        SpawnBoss(normalBossEnemyPrefab);
                        SpawnBoss(fastBossEnemyPrefab);
                        SpawnBoss(rangedBossEnemyPrefab);
                        break;
                    case 150:
                        SpawnBoss(rangedBossEnemyPrefab);
                        SpawnBoss(wandererBossEnemyPrefab);
                        break;
                    case 180:
                        SpawnBoss(wandererRangedBossEnemyPrefab);
                        SpawnBoss(wandererBossEnemyPrefab);
                        break;
                    case 210:
                        SpawnBoss(wandererRangedBossEnemyPrefab);
                        SpawnBoss(wandererBossEnemyPrefab);
                        SpawnBoss(wandererExploderBossEnemyPrefab);
                        break;
                    case 240:
                        SpawnBoss(rangedBossEnemyPrefab);
                        SpawnBoss(rangedBossEnemyPrefab);
                        SpawnBoss(rangedBossEnemyPrefab);
                        break;
                    case 270:
                        SpawnBoss(bigBossEnemyPrefab);
                        break;
                    case 300:
                        SpawnBoss(normalBossEnemyPrefab);
                        SpawnBoss(fastBossEnemyPrefab);
                        SpawnBoss(bigBossEnemyPrefab);
                        SpawnBoss(rangedBossEnemyPrefab);
                        SpawnBoss(chargerBossEnemyPrefab);
                        SpawnBoss(wandererBossEnemyPrefab);
                        SpawnBoss(wandererRangedEnemyPrefab);
                        SpawnBoss(wandererExploderBossEnemyPrefab);
                        break;
                        
                }
            }
               
            
            chestManager.ReduceChestSpawnTime();
            
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
            
            Destroy(enemy.gameObject);
        }

        /// <summary>
        /// Spawns all boss enemies defined in the current block.
        /// </summary>
        public void SpawnBoss(EnemyController bossEnemyPrefab)
        {
            progressPaused = true;
            var enemy = Instantiate(bossEnemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            enemy.playerTarget = playerTarget;
            enemy.enemyManager = this;
            enemy.ApplyMultipliers(healthMultiplier, damageMultiplier);
            enemy.isBoss = true;
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

            if (activeBosses.Count == 0)
            {
                ChestManager.Instance.SpawnBossChest(boss.transform.position);
                
                enemyKillProgressCount++;
                progressPaused = false;

                if (enemyKillProgressCount >= 300)
                {
                    GameManager.Instance.WinGame();
                }
            }
            
            Destroy(boss.gameObject);
        }

        /// <summary>
        /// Generates a random spawn point around the player within a specified radius.
        /// </summary>
        /// <returns>A random Vector3 position.</returns>
        private Vector3 GetRandomSpawnPoint()
        {
            // TODO: Fix this.
            float spawnRadius = GameManager.Instance.levelBounds.x * 0.5f; // Adjust as needed
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = playerTarget.position + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnRadius;
            return spawnPos;
        }

        public void Reset()
        {
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
            _timeSinceLastSpawn = 0f;
        }
    }
}
