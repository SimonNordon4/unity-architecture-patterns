using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "EnemySpawnBlock", menuName = "Prototype/EnemySpawnBlock", order = 1)]
public class EnemySpawnBlock : ScriptableObject
{
    public Vector2 healthMultiplier = new Vector2(1, 1);
    public Vector2 damageMultiplier = new Vector2(1, 1);
    public Vector2Int bossChestTier = new Vector2Int(1, 2);
    public Vector2Int bossChestChoices = new Vector2Int(3, 3);
    
    public List<EnemySpawnWave> spawnWaves = new();
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawnBlock))]
public class EnemySpawnBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var block = (EnemySpawnBlock) target;
        
        EditorGUILayout.BeginVertical("box");
        var totalSeconds = block.spawnWaves.Sum(wave => wave.blockTime);
        var minutes = (int)(totalSeconds / 60);
        var seconds = (int)(totalSeconds % 60);
        var formattedTime = $"{minutes}m {seconds}s";

        EditorGUILayout.LabelField($"Block Time: {formattedTime}");
        EditorGUILayout.LabelField($"Total Enemies: {block.spawnWaves.Sum(wave => wave.totalEnemies)}");
        EditorGUILayout.EndVertical();
        
        base.OnInspectorGUI();
    }
}

#endif