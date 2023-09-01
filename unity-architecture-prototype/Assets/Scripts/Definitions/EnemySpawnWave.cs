using System;
using System.Collections.Generic;
using Definitions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
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
    public EnemySpawnAction bossAction;
    
    #if UNITY_EDITOR
    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
        _parentBlock = GetParentBlock();
        _isParentNull = _parentBlock == null;
    }
    

    #region Stat Tracking

    private List<EnemyType> _enemyTypes = new();
    private EnemySpawnBlock _parentBlock;
    private bool _isParentNull = false;
    
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
            parentBlock = _isParentNull ? "None" : _parentBlock.name
        };
    }

    private EnemySpawnBlock GetParentBlock()
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemySpawnBlock");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemySpawnBlock block = AssetDatabase.LoadAssetAtPath<EnemySpawnBlock>(path);

            foreach (var spawnWave in block.spawnWaves)
            {
                if (spawnWave == this)
                {
                    return block; 
                }
            }
        }
        return null; 
    }

    private List<EnemyType> GetEnemyTypesList()
    {
        var parentBlock = _parentBlock;

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
                
                var healthMultiplier = _isParentNull ? 1 : (parentBlock.healthMultiplier.x + parentBlock.healthMultiplier.y) / 2;
                health += Mathf.RoundToInt(enemyController.currentHealth * action.numberOfEnemiesToSpawn * healthMultiplier);
                
                var damageMultiplier = _isParentNull ? 1 : (parentBlock.damageMultiplier.x + parentBlock.damageMultiplier.y) / 2;
                damage += Mathf.RoundToInt(enemyController.damageAmount * action.numberOfEnemiesToSpawn * damageMultiplier);
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