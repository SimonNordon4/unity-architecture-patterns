using System;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class EnemyBlock : MonoBehaviour
    {
        [Header("Block Configuration")]
        public string blockName = "Unnamed Block"; // Optional: For identification
        public int enemiesToKill = 200;
        public Vector2Int maxEnemiesAliveRange = new(5, 10);
        public AnimationCurve maxEnemiesAliveCurve = AnimationCurve.Linear(0, 1, 1, 1);
        
        [Header("Enemy Types")]
        public EnemyController[] enemyPrefabs;
        public AnimationCurve[] enemySpawnWeightCurves;
        
        [Header("Boss Enemies")]
        public EnemyController[] bossEnemies;

        

        [Header("Health & Damage Multipliers")]
        public Vector2Int healthMultiplierRange = new(1, 4);
        public Vector2Int damageMultiplierRange = new(1, 4);
        [Tooltip("Curve to evaluate health multiplier based on progress (0 to 1).")]
        public AnimationCurve healthMultiplierCurve;
        [Tooltip("Curve to evaluate damage multiplier based on progress (0 to 1).")]
        public AnimationCurve damageMultiplierCurve;

        // Internal tracking of enemies killed
        [HideInInspector]
        public int enemiesKilled = 0;

        public int GetMaxEnemiesAlive()
        {
            return (int)Mathf.Lerp(maxEnemiesAliveRange.x, maxEnemiesAliveRange.y, maxEnemiesAliveCurve.Evaluate(enemiesKilled / (float)enemiesToKill));
        }

        /// <summary>
        /// Selects an EnemyController based on weighted probabilities defined by AnimationCurves.
        /// </summary>
        /// <returns>A randomly selected EnemyController prefab.</returns>
        public EnemyController GetEnemy()
        {
            // Ensure that the arrays are properly configured
            if (enemyPrefabs == null || enemySpawnWeightCurves == null)
            {
                Debug.LogError($"Enemy prefabs or spawn weight curves are not assigned for block: {blockName}.");
                return null;
            }

            if (enemyPrefabs.Length != enemySpawnWeightCurves.Length)
            {
                Debug.LogError($"Enemy prefabs and spawn weight curves arrays must be of the same length for block: {blockName}.");
                return null;
            }

            // Calculate progress, clamped between 0 and 1
            float progress = Mathf.Clamp01((float)enemiesKilled / enemiesToKill);

            // Calculate weights based on the spawn weight curves
            float[] weights = new float[enemySpawnWeightCurves.Length];
            float totalWeight = 0f;

            for (int i = 0; i < enemySpawnWeightCurves.Length; i++)
            {
                float weight = enemySpawnWeightCurves[i].Evaluate(progress);
                // Ensure that weights are non-negative
                weight = Mathf.Max(weight, 0f);
                weights[i] = weight;
                totalWeight += weight;
            }

            // Handle the case where total weight is zero to avoid division by zero
            if (totalWeight == 0f)
            {
                Debug.LogWarning($"Total spawn weight is zero for block: {blockName}. Defaulting to the first enemy prefab.");
                return enemyPrefabs[0];
            }

            // Generate a random value between 0 and totalWeight
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            // Select the enemy based on the random value and cumulative weights
            for (int i = 0; i < weights.Length; i++)
            {
                cumulativeWeight += weights[i];
                if (randomValue <= cumulativeWeight)
                {
                    return enemyPrefabs[i];
                }
            }

            // Fallback in case of floating point inaccuracies
            Debug.LogWarning($"Failed to select an enemy for block: {blockName} based on weights. Defaulting to the last enemy prefab.");
            return enemyPrefabs[enemyPrefabs.Length - 1];
        }

        /// <summary>
        /// Evaluates the health multiplier based on current progress and maps it to the defined range.
        /// </summary>
        /// <returns>Health multiplier as an integer.</returns>
        public float GetHealthMultiplier()
        {
            float progress = Mathf.Clamp01((float)enemiesKilled / enemiesToKill);
            float curveValue = healthMultiplierCurve.Evaluate(progress);
            // Assuming the curve outputs values between 0 and 1
            return Mathf.Lerp(healthMultiplierRange.x, healthMultiplierRange.y, curveValue);
        }

        /// <summary>
        /// Evaluates the damage multiplier based on current progress and maps it to the defined range.
        /// </summary>
        /// <returns>Damage multiplier as an integer.</returns>
        public float GetDamageMultiplier()
        {
            float progress = Mathf.Clamp01((float)enemiesKilled / enemiesToKill);
            float curveValue = damageMultiplierCurve.Evaluate(progress);
            // Assuming the curve outputs values between 0 and 1
            return Mathf.Lerp(damageMultiplierRange.x, damageMultiplierRange.y, curveValue);
        }
    }
}
