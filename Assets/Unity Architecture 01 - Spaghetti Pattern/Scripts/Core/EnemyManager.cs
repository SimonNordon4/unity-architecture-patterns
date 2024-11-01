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

        [Header("Spawn Settings")]
        public float baseSpawnRate = 1f;
        private float _timeSinceLastSpawn = 0f;

        [Header("Enemy Blocks Configuration")]
        public EnemyBlock[] enemyBlocks;
        public int currentBlockIndex = 0;

        // Tracking active enemies and bosses
        public int enemiesAlive = 0;
        private int bossesAlive = 0;

        // Lists to keep track of active enemies and bosses
        public List<EnemyController> activeEnemies = new();
        public List<EnemyController> activeBosses = new();

        private void Update()
        {
            if (!gameManager.isGameActive) return;

            // Increment the spawn timer
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn > GetSpawnRate())
            {
                _timeSinceLastSpawn = 0f;
                SpawnEnemy();
            }
        }

        /// <summary>
        /// Spawns a regular enemy based on the current block's configuration.
        /// </summary>
        private void SpawnEnemy()
        {
            if (currentBlockIndex >= enemyBlocks.Length)
            {
                Debug.LogWarning("All blocks have been completed. No more enemies to spawn.");
                return;
            }

            var currentBlock = enemyBlocks[currentBlockIndex];

            if (enemiesAlive >= currentBlock.GetMaxEnemiesAlive())
            {
                // Too many enemies alive; skip spawning
                return;
            }

            var enemyPrefab = currentBlock.GetEnemy();

            if (enemyPrefab == null)
            {
                Debug.LogError($"Failed to get enemy prefab for block: {currentBlock.blockName}");
                return;
            }

            var enemy = Instantiate(enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            enemy.playerTarget = playerTarget;
            enemy.enemyManager = this;

            // Apply health and damage multipliers
            float healthMultiplier = currentBlock.GetHealthMultiplier();
            float damageMultiplier = currentBlock.GetDamageMultiplier();
            enemy.ApplyMultipliers(healthMultiplier, damageMultiplier);

            enemiesAlive++;
            activeEnemies.Add(enemy);

            Console.Log($"Spawn Enemy. alive: {enemiesAlive}," +
                        $" type: {enemyPrefab.gameObject.name}," +
                        $" health mult: {healthMultiplier:F1}," +
                        $" damage mult {damageMultiplier:F1}",
                LogFilter.Enemy, this);
        }

        /// <summary>
        /// Called when a regular enemy dies.
        /// </summary>
        /// <param name="enemy">The enemy that died.</param>
        public void EnemyDied(EnemyController enemy)
        {
            chestManager.ReduceChestSpawnTime();
            
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }

            enemiesAlive--;
            enemyBlocks[currentBlockIndex].enemiesKilled++;

            if (enemyBlocks[currentBlockIndex].enemiesKilled >= enemyBlocks[currentBlockIndex].enemiesToKill)
            {
                SpawnBoss();
            }
            
        }

        /// <summary>
        /// Spawns all boss enemies defined in the current block.
        /// </summary>
        public void SpawnBoss()
        {
            var currentBlock = enemyBlocks[currentBlockIndex];

            if (currentBlock.bossEnemies == null || currentBlock.bossEnemies.Length == 0)
            {
                Debug.LogWarning($"No boss enemies assigned for block: {currentBlock.blockName}");
                ProceedToNextBlock();
                return;
            }

            foreach (var bossPrefab in currentBlock.bossEnemies)
            {
                var boss = Instantiate(bossPrefab, GetRandomSpawnPoint(), Quaternion.identity);
                boss.playerTarget = playerTarget;
                boss.enemyManager = this;

                // Apply health and damage multipliers
                float healthMultiplier = currentBlock.GetHealthMultiplier();
                float damageMultiplier = currentBlock.GetDamageMultiplier();
                boss.ApplyMultipliers(healthMultiplier, damageMultiplier);

                bossesAlive++;
                activeBosses.Add(boss);
            }
            
            Console.Log("EnemyManager.SpawnBoss()", LogFilter.Enemy, this);

            // Optionally, stop spawning regular enemies when bosses are active
            // This can be handled by checking if bossesAlive > 0 before spawning
        }

        /// <summary>
        /// Called when a boss dies.
        /// </summary>
        /// <param name="boss">The boss that died.</param>
        public void BossDied(EnemyController boss)
        {
            Console.Log("EnemyManager.BossDied()", LogFilter.Enemy, this);
            if (activeBosses.Contains(boss))
            {
                activeBosses.Remove(boss);
            }

            bossesAlive--;

            if (bossesAlive <= 0)
            {
                ProceedToNextBlock();
            }
        }

        /// <summary>
        /// Handles the transition to the next block.
        /// </summary>
        private void ProceedToNextBlock()
        {
            Console.Log($"EnemyManager.ProceedToNextBlock() current: {currentBlockIndex} next: {currentBlockIndex+1}", LogFilter.Enemy, this);
            currentBlockIndex++;

            if (currentBlockIndex >= enemyBlocks.Length)
            {
                Debug.Log("All blocks completed! Player wins!");
                gameManager.WinGame();
            }
            else
            {
                Debug.Log($"Block {currentBlockIndex + 1} started.");
                // Reset tracking variables for the new block
                enemiesAlive = 0;
                enemyBlocks[currentBlockIndex].enemiesKilled = 0;
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
            if (currentBlockIndex >= enemyBlocks.Length)
            {
                return baseSpawnRate;
            }

            float currentMaxEnemies = enemyBlocks[currentBlockIndex].GetMaxEnemiesAlive();
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
            currentBlockIndex = 0;
            _timeSinceLastSpawn = 0f;

            // Reset enemy blocks' kill counters
            foreach (var block in enemyBlocks)
            {
                block.enemiesKilled = 0;
            }
        }
    }
}
