using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Actors
{
    public class ActorStats : ActorComponent
    {
        [SerializeField] private ActorStatsDefinition definition;
        public readonly Dictionary<StatType, Stat> Map = new();
        
        public void Initialize(ActorStatsDefinition newDefinition)
        {
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                Map[type] = newDefinition.GetStatByType(type);
            }
        }

        private void Awake()
        {
            if (definition != null)
            {
                Initialize(definition);
            }
        }
    }
    
#if UNITY_EDITOR

    [CustomEditor(typeof(ActorStats))]
    public class ActorStatsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            base.OnInspectorGUI();

            // Get the target object
            ActorStats actorStats = (ActorStats)target;

            if (actorStats.Map == null) return;

            foreach (var keyValuePair in actorStats.Map)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(keyValuePair.Key.ToString());

                // Assuming Stat has a property InitialValue of type int
                int newInitialValue = EditorGUILayout.IntField((int)keyValuePair.Value.initialValue);
                if (Math.Abs(newInitialValue - keyValuePair.Value.initialValue) > 0.01f)
                {
                    keyValuePair.Value.initialValue = newInitialValue;
                    EditorUtility.SetDirty(target); // Mark the object as dirty to enable saving the changed value
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}