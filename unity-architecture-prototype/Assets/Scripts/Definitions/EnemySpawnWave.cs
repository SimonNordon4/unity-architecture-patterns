using System.Collections.Generic;
using Definitions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif
[CreateAssetMenu(fileName = "EnemySpawnWave", menuName = "Prototype/EnemySpawnBlock", order = 1)]
public class EnemySpawnWave : ScriptableObject
{
    public EnemyManager enemyManager;
    public List<EnemySpawnAction> enemySpawnActions = new();

    [Header("Base Stats")]
    public int totalEnemies = 100;
    public float blockTime = 300f;
    public AnimationCurve spawnRateCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public EnemySpawnAction bossAction;

    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
    }
}

// #if UNITY_EDITOR
//
// [CustomEditor(typeof(EnemySpawnWave))]
// public class EnemySpawnBlockEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//
//         var block = target as EnemySpawnWave;
//         
//         var enemyStats = new List<EnemyTypeStats>();
//         int totalProbability = 0;
//         foreach (var spawnAction in block.enemySpawnActions)
//         {
//             totalProbability += spawnAction.spawnWeight * spawnAction.numberOfEnemiesToSpawn;
//             
//             var prefabName = spawnAction.enemyPrefab.name;
//             // check if enemyStats already contains this name
//             var enemyType = enemyStats.FirstOrDefault(x => x.name == prefabName);
//             if (enemyType != null)
//             {
//                 enemyType.actions.Add(spawnAction);
//             }
//             else
//             {
//                 var newEnemyType = new EnemyTypeStats();
//                 newEnemyType.name = prefabName;
//                 newEnemyType.actions = new List<EnemySpawnAction>();
//                 newEnemyType.actions.Add(spawnAction);
//                 enemyStats.Add(newEnemyType);
//             }
//         }
//         
//         foreach (var enemyStat in enemyStats)
//         {
//             var probability = 0;
//             var health = 0;
//             var damage = 0;
//             var enemyToSpawn = 0;
//             foreach (var action in enemyStat.actions)
//             {
//                 enemyToSpawn += action.numberOfEnemiesToSpawn;
//                 probability += action.spawnWeight * action.numberOfEnemiesToSpawn;
//                 health += action.health * action.numberOfEnemiesToSpawn;
//                 damage += action.damage * action.numberOfEnemiesToSpawn;
//             }
//             enemyStat.probability = (float)probability / totalProbability;
//             enemyStat.totalEnemies = (int)(enemyStat.probability * block.totalEnemies);
//             enemyStat.enemiesPerSecond = (float)enemyStat.totalEnemies / block.blockTime;
//             
//             enemyStat.averageHealth = (float)health / enemyToSpawn;
//             enemyStat.averageDamage = (float)damage / enemyToSpawn;
//             
//             enemyStat.totalHealth = (int)(enemyStat.averageHealth * enemyStat.totalEnemies);
//             enemyStat.totalDamage = (int)(enemyStat.averageDamage * enemyStat.totalEnemies);
//
//         }
//         
//         // now we want to publish total stats.
//         EditorGUILayout.BeginVertical("box");
//         EditorGUILayout.LabelField($"Total",EditorStyles.boldLabel);
//         EditorGUILayout.LabelField($"Enemies Per Second: {block.totalEnemies / block.blockTime:F1}");
//         
//         
//         EditorGUILayout.LabelField($"Total Health: {enemyStats.Sum(x => x.totalHealth)}");
//         
//         EditorGUILayout.BeginHorizontal();
//         EditorGUILayout.LabelField($"Player DPS: {enemyStats.Sum(x => x.totalHealth) / block.blockTime:F1}");
//         // calculate how much damage we would take in 5 hits.
//         var totalDamage = enemyStats.Sum(x => x.totalDamage);
//         var averageDamagePerHit = totalDamage / block.totalEnemies;
//         var fiveHitCombo = averageDamagePerHit * 5;
//         EditorGUILayout.LabelField("Player Health: " + fiveHitCombo);
//         EditorGUILayout.EndHorizontal();
//         EditorGUILayout.EndVertical();
//
//         foreach (var enemyStat in enemyStats)
//         {
//             // draw a box
//             EditorGUILayout.BeginVertical("box");
//             
//             EditorGUILayout.LabelField($"{enemyStat.name}",EditorStyles.boldLabel);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField($"Total Enemies: {enemyStat.totalEnemies}");
//             EditorGUILayout.LabelField($"Probability: {enemyStat.probability * 100}%");
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField($"Total Health: {enemyStat.totalHealth}");
//             EditorGUILayout.LabelField($"Average Health: {enemyStat.averageHealth}");
//             EditorGUILayout.EndHorizontal();
//             
//             EditorGUILayout.EndVertical();
//         }
//         
//         base.OnInspectorGUI();
//     }
//     
//     private class EnemyTypeStats
//     {
//         public string name;
//         
//         public float probability;
//         public int totalEnemies;
//         public float enemiesPerSecond;
//         
//         public float averageHealth;
//         public float averageDamage;
//         
//         public int totalHealth;
//         public int totalDamage;
//         
//         public List<EnemySpawnAction> actions;
//     }
// }
//
// #endif  