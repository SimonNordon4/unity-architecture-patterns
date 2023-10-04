using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Classic.Utility;
using UnityEditor;
#endif

namespace Classic.App
{
    [CreateAssetMenu(fileName = "AchievementDefinition", menuName = "Classic/AchievementDefinition")]
    public class AchievementDefinition : ScriptableObject
    {
        public AchievementId id;
        public string uiName;
        public int goal;
        public int rewardGold;
        // only relevant for stat achievements.
        public StatType statType;
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(AchievementDefinition))]
    public class AchievementDefinitionEditor : Editor
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
    #endif
}