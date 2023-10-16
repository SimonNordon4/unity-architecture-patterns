using Classic.Actor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Debugging
{
    public class ActorStatsDebugger : DebugComponent
    {
        [SerializeField] private ActorStats stats;
        [SerializeField] private StatType statType;
        [SerializeField] private float value;
        
        public void SetStat()
        {
            Debug.Log($"Setting stat {statType} to {value}");
            stats.Map[statType].value = value;
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