using System;
using Classic.Enemies;
using UnityEditor;
using UnityEngine;

namespace Classic.Editor
{
    [CustomEditor(typeof(EnemySpawner))]
    public class EnemySpawnerEditor : UnityEditor.Editor
    {
        private EnemyDefinition _definition;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spawner = (EnemySpawner) target;
            if (GUILayout.Button("Spawn Enemy"))
            {
                spawner.SpawnEnemy(spawner.testDefinition);
            }
        }
    }
}