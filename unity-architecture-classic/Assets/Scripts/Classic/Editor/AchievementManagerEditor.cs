#if UNITY_EDITOR
using System.Collections.Generic;
using Classic.App;
using UnityEditor;
using UnityEngine;

namespace Classic.Editor
{
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var achievementManager = (AchievementManager) target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Reset All"))
            {
                achievementManager.ResetAll();
            }

            if (GUILayout.Button("Get Achievement Definitions"))
            {
                // find all achievement definitions in the project and add them to the list.
                var achievementDefinitions = new List<AchievementDefinition>();
                var guids = AssetDatabase.FindAssets("t:AchievementDefinition");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<AchievementDefinition>(path);
                    achievementDefinitions.Add(asset);
                    achievementManager.SetDefinitions(achievementDefinitions);
                }
                
            }
        }
    }
}
#endif