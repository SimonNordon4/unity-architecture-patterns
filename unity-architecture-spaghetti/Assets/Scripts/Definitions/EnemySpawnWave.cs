using System;
using System.Collections.Generic;
using Definitions;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "EnemySpawnWave", menuName = "Prototype/EnemySpawnWave", order = 1)]
public class EnemySpawnWave : ScriptableObject
{
    public EnemyManager enemyManager;
    public List<EnemySpawnAction> enemySpawnActions = new();

    [Header("Base Stats")]
    public int totalEnemies = 100;
    public float blockTime = 300f;
    public AnimationCurve spawnRateCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public List<EnemySpawnAction> eliteAction;
    
    public Vector2 healthMultiplier = new Vector2(1, 1);
    public Vector2 damageMultiplier = new Vector2(1, 1);
    public Vector2Int bossChestTier = new(1, 2);
    public Vector2Int bossChestChoices = new(3, 3);

    [Header("Spawn Rate")] public int idealEnemiesAlive = 5;
    public int decay = 5;
    
    public int TotalEnemyCount()
    {
        var bossEnemies = eliteAction.Sum(action => action.numberOfEnemiesToSpawn);
        return totalEnemies + bossEnemies;
    }
    
    #if UNITY_EDITOR
    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
    }
    #region Stat Tracking

    private EnemySpawnBlock _parentBlock;
    
    [Serializable]
    public class EnemyType
    {
        public string name;
        public int totalEnemies;
        public float probability;
        public float averageHealth;
        public float averageDamage;
        public int totalHealth;
        public int totalDamage;
        public List<EnemySpawnAction> actions;
    }

    [Serializable]
    public struct Data 
    {
        public List<EnemyType> enemyTypes;
        public int totalEnemies;
        public float blockTime;
        public float enemiesPerSecond;
        public int totalHealth;
        public int averageDamage;
        public float idealDps;
        public int idealHealth;
        public string parentBlock;
    }

    public Data GetSpawnWaveData()
    {
  
        var enemyTypes = GetEnemyTypesList();
        var totalHealth = enemyTypes.Sum(x => x.totalHealth);
        var averageDamage = Mathf.RoundToInt(enemyTypes.Sum(x => x.averageDamage));
        
        return new Data
        {
            enemyTypes = enemyTypes,
            totalEnemies = totalEnemies,
            blockTime = blockTime,
            enemiesPerSecond = totalEnemies / blockTime,
            totalHealth = totalHealth,
            averageDamage = Mathf.RoundToInt(enemyTypes.Sum(x => x.averageDamage)),
            idealDps = totalHealth / blockTime,
            idealHealth = Mathf.RoundToInt(averageDamage * 10),
        };
    }

    private List<EnemyType> GetEnemyTypesList()
    {
        var enemyStats = new List<EnemyType>();
        int totalProbability = 0;
        foreach (var spawnAction in enemySpawnActions)
        {
            totalProbability += spawnAction.spawnWeight * spawnAction.numberOfEnemiesToSpawn;

            if (spawnAction.enemyPrefab == null) return new List<EnemyType>();
            
            var prefabName = spawnAction.enemyPrefab.name;
            // check if enemyStats already contains this name
            var enemyType = enemyStats.FirstOrDefault(x => x.name == prefabName);
            if (enemyType != null)
            {
                enemyType.actions.Add(spawnAction);
            }
            else
            {
                var newEnemyType = new EnemyType();
                newEnemyType.name = prefabName;
                newEnemyType.actions = new List<EnemySpawnAction>();
                newEnemyType.actions.Add(spawnAction);
                enemyStats.Add(newEnemyType);
            }
        }
        foreach (var enemyStat in enemyStats)
        {
            var probability = 0;
            var health = 0;
            var damage = 0;
            var enemyToSpawn = 0;
            foreach (var action in enemyStat.actions)
            {
                enemyToSpawn += action.numberOfEnemiesToSpawn;
                probability += action.spawnWeight * action.numberOfEnemiesToSpawn;
                var enemyController = action.enemyPrefab.GetComponent<EnemyController>();
                
                var healthMult = (this.healthMultiplier.x + this.healthMultiplier.y) / 2;
                health += Mathf.RoundToInt(enemyController.currentHealth * action.numberOfEnemiesToSpawn * healthMult);
                
                var damageMult = (this.damageMultiplier.x + this.damageMultiplier.y) / 2;
                damage += Mathf.RoundToInt(enemyController.damageAmount * action.numberOfEnemiesToSpawn * damageMult);
            }

            enemyStat.probability = (float)probability / totalProbability;
            enemyStat.totalEnemies = (int)(enemyStat.probability * totalEnemies);

            enemyStat.averageHealth = (float)health / enemyToSpawn;
            enemyStat.averageDamage = (float)damage / enemyToSpawn;

            enemyStat.totalHealth = (int)(enemyStat.averageHealth * enemyStat.totalEnemies);
            enemyStat.totalDamage = (int)(enemyStat.averageDamage * enemyStat.totalEnemies);
        }
        
        return enemyStats;
    }

    #endregion
    #endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(EnemySpawnWave))]
public class EnemySpawnWaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var block = target as EnemySpawnWave;
        if (block != null)
        {
            var blockStats = block.GetSpawnWaveData();
            
            var enemyStats = blockStats.enemyTypes;
            // now we want to publish total stats.
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Overview",EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Parent Block: {blockStats.parentBlock}");
            EditorGUILayout.LabelField($"Enemies Per Second: {block.totalEnemies / block.blockTime:F1}");
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Total Health: {blockStats.totalHealth}");
            EditorGUILayout.LabelField($"Average Damage: {blockStats.averageDamage}");
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Player DPS: {enemyStats.Sum(x => x.totalHealth) / block.blockTime:F1}");
            // calculate how much damage we would take in 5 hits.
            var totalDamage = enemyStats.Sum(x => x.totalDamage);
            var averageDamagePerHit = totalDamage / block.totalEnemies;
            var fiveHitCombo = averageDamagePerHit * 5;
            EditorGUILayout.LabelField("Player Health: " + fiveHitCombo);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            foreach (var enemyStat in enemyStats)
            {
                // draw a box
                EditorGUILayout.BeginVertical("box");
            
                EditorGUILayout.LabelField($"{enemyStat.name}",EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Total Enemies: {enemyStat.totalEnemies}");
                EditorGUILayout.LabelField($"Probability: {enemyStat.probability * 100}%");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Total Health: {enemyStat.totalHealth}");
                EditorGUILayout.LabelField($"Average Health: {enemyStat.averageHealth}");
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.EndVertical();
            }
        }

        base.OnInspectorGUI();
    }
}

#endif  