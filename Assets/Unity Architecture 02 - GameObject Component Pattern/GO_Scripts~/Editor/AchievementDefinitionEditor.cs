using GameObjectComponent.Utility;
using UnityEditor;
using UnityEngine;

namespace GameObjectComponent.App
{
    [CustomEditor(typeof(AchievementDefinition))]
    public class AchievementDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var achievement = (AchievementDefinition) target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Set Name"))
            {
                var achievementName = SurvivorsUtil.CamelCaseToString(achievement.id.ToString());
                // split
                var names = achievementName.Split(' ');
                var finalName = $"{names[0]} {achievement.goal} {names[1]}";
                // rename this asset to final name
                
                achievement.uiName = finalName;
                
                var path = AssetDatabase.GetAssetPath(achievement);
                AssetDatabase.RenameAsset(path, finalName);
                AssetDatabase.SaveAssets();
            }
            
            if (GUILayout.Button("Set Stat Name"))
            {
                var achievementName = SurvivorsUtil.CamelCaseToString(achievement.id.ToString());
                // split
                var names = achievementName.Split(' ');
                var finalName = $"{names[0]} {achievement.goal} {SurvivorsUtil.CamelCaseToString(achievement.statType.ToString())}";
                // rename this asset to final name
                
                achievement.uiName = finalName;
                
                var path = AssetDatabase.GetAssetPath(achievement);
                AssetDatabase.RenameAsset(path, finalName);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
