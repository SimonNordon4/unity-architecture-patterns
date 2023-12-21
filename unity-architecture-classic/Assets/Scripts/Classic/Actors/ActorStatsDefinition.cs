using System;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Classic.Actors
{
    [CreateAssetMenu(menuName = "Classic/ActorStatsDefinition")]
    public class ActorStatsDefinition : ScriptableObject
    {
        [field: SerializeField] public List<Stat> stats { get; private set; } = new();
        
        public Stat GetStatByType(StatType type)
        {
            foreach (var stat in stats)
            {
                if (stat.type == type)
                {
                    // return a copy of the stat
                    return new Stat(stat);
                }
            }

            return new Stat(type);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorStatsDefinition))]
    public class ActorStatsDefinitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            ActorStatsDefinition statsDef = (ActorStatsDefinition)target;
            
            if (GUILayout.Button("Generate StatDefinitions"))
            {
                GenerateStatDefinitions(statsDef);
            }
        }

        void GenerateStatDefinitions(ActorStatsDefinition statsDef)
        {
            statsDef.stats.Clear();
            
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                var newStat = new Stat(type);
                statsDef.stats.Add(newStat);
            }
            EditorUtility.SetDirty(statsDef);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}