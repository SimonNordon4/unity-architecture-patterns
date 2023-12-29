using System;
using GameObjectComponent.App;
using GameObjectComponent.Utility;
using UnityEditor;
using UnityEngine;

namespace GameObjectComponent.Editor
{
    [CustomEditor(typeof(StatisticDefinition))]
    public class StatisticDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var statistic = (StatisticDefinition) target;
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Set Name"))
            {
                Rename(statistic);
            }

            if (GUILayout.Button("Create Statistics"))
            {
                // Create a new statistic scriptable object for each statistic type and apply the name function to it.
                // When it comes to the 'HighestStat' Statistic, create a new statistic for each stat type that has isStat set to true.
                // Replace the name with the stat so that HighestStat will become HighestFireRate etc.
                
                foreach (StatisticType statisticType in Enum.GetValues(typeof(StatisticType)))
                {
                    if (statisticType != StatisticType.HighestStat)
                    {
                        CreateStatistic(statisticType);
                    }
                    else
                    {
                        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
                        {
                            var highestStatName = "Highest " + statType.ToString();
                            CreateStatistic(statisticType, highestStatName, statType);
                        }
                    }
                }
            }
        }
        
        private void CreateStatistic(StatisticType statisticType, string nameOverride = null, StatType? statType = null)
        {
            var statistic = ScriptableObject.CreateInstance<StatisticDefinition>();
            statistic.name = nameOverride ?? SurvivorsUtil.CamelCaseToString(statisticType.ToString());
            statistic.statisticType = statisticType;
            statistic.statType = statType ?? default;

            AssetDatabase.CreateAsset(statistic, "Assets/ScriptableObjects/Statistics/" + statistic.name + ".asset");
        }

        public void Rename(StatisticDefinition statistic)
        {
            var statisticName = SurvivorsUtil.CamelCaseToString(statistic.statisticType.ToString());
            statistic.name = statisticName;
                
            var path = AssetDatabase.GetAssetPath(statistic);
            AssetDatabase.RenameAsset(path, statisticName);
            AssetDatabase.SaveAssets();
        }
    }
}