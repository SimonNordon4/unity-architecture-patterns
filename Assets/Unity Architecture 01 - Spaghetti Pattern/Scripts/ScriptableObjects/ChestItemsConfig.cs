using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityArchitecture.SpaghettiPattern
{
    [CreateAssetMenu(fileName = "ChestItemsConfig", menuName = "UnityArchitecture/SpaghettiPattern/ChestItemsConfig", order = 1)]
    public class ChestItemsConfig : ScriptableObject
    {
        public List<ChestItem> chestItems;

#if UNITY_EDITOR
        [ContextMenu("Get All")]
        public void GetAll()
        {
            // Get all chest items in the project
            var guids = AssetDatabase.FindAssets("t:ChestItem");
            chestItems = new List<ChestItem>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var chestItem = AssetDatabase.LoadAssetAtPath<ChestItem>(path);
                chestItems.Add(chestItem);
            }
            // save
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ChestItemsConfig))]
    public class ChestItemsConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Total Stats", EditorStyles.boldLabel);

            var statDictionary = new Dictionary<StatType, float>();

            foreach (var statType in System.Enum.GetValues(typeof(StatType)))
            {
                statDictionary.Add((StatType)statType, 0);
            }

            var config = target as ChestItemsConfig;

            foreach (var chestItem in config.chestItems)
            {
                foreach (var modifier in chestItem.modifiers)
                {
                    statDictionary[modifier.statType] += modifier.modifierValue;
                }
            }

            foreach (var stat in statDictionary)
            {
                // if the number is positive make it green, red for negative, grey for 0
                var red = new Color(1, 0.5f, 0.5f);
                var green = new Color(0.5f, 1, 0.5f);
                var gray = new Color(0.5f, 0.5f, 0.5f);
                var color = stat.Value > 0 ? green : stat.Value < 0 ? red : gray;
                GUI.color = color;
                EditorGUILayout.LabelField(stat.Key.ToString(), stat.Value.ToString());
            }
        }
    }
#endif
}