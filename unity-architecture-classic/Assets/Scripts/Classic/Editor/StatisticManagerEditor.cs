using System.Collections.Generic;
using Classic.App;
using UnityEngine;
using UnityEditor;

namespace Classic.Editor
{
    [CustomEditor(typeof(StatisticsManager))]
    public class StatisticManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var statisticManager = (StatisticsManager) target;
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Reset All"))
            {
                statisticManager.ResetAll();
            }
            
            if (GUILayout.Button("Get Statistic Definitions"))
            {
                // find all statistic definitions in the project and add them to the list.
                var statisticDefinitions = new List<StatisticDefinition>();
                var guids = AssetDatabase.FindAssets("t:StatisticDefinition");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<StatisticDefinition>(path);
                    statisticDefinitions.Add(asset);
                    statisticManager.SetDefinitions(statisticDefinitions);
                }
            }
        }
    }
}