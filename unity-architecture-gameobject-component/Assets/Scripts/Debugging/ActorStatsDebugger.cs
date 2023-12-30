using GameplayComponents.Combat;
using GameplayComponents.Actor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameObjectComponent.Debugging
{
    public class ActorStatsDebugger : DebugComponent
    {
        [SerializeField] private Stats stats;
        [SerializeField] private StatType statType;
        [SerializeField] private float value;
        
        public void SetStat()
        {
            Debug.Log($"Setting stat {statType} to {value}");
            stats.GetStat(statType).value = value;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorStatsDebugger))]
    public class ActorStatsDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (ActorStatsDebugger)target;
            if (GUILayout.Button("Set Stat"))
            {
                myScript.SetStat();
            }
        }
    }
    
    #endif
}