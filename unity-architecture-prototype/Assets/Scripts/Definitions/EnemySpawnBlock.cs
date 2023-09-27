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
    public float goldMultiplier = 1;
    public int tier;

    [Inline]
    public List<EnemySpawnWave> spawnWaves = new();

    public int TotalEnemyCount()
    {
        var normalEnemies = spawnWaves.Sum(wave => wave.totalEnemies);
        var bossEnemies = spawnWaves.Sum(wave => wave.eliteAction.Sum(action => action.numberOfEnemiesToSpawn));
        return normalEnemies + bossEnemies;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawnBlock))]
public class EnemySpawnBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var block = (EnemySpawnBlock)target;

        EditorGUILayout.BeginVertical("box");
        if (block == null || block.spawnWaves == null) return;
        var totalSeconds = block.spawnWaves.Sum(wave =>
        {
            if (wave == null)
                return 0;
            return wave.blockTime;
        });
        var minutes = (int)(totalSeconds / 60);
        var seconds = (int)(totalSeconds % 60);
        var formattedTime = $"{minutes}m {seconds}s";

        var totalEnemies = block.spawnWaves.Sum(wave =>
        {
            if (wave == null)
                return 0;
            return wave.totalEnemies;
        });

        EditorGUILayout.LabelField($"Block Time: {formattedTime}");
        EditorGUILayout.LabelField($@"Total Enemies: {totalEnemies}");

        EditorGUILayout.LabelField($"Base Gold: {block.goldMultiplier * totalEnemies}");

        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();
    }
}

#endif