using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityArchitecture.SpaghettiPattern
{
    public class ChestItems : MonoBehaviour
    {
        public string chestTableName = "ChestItems";
        public int tier = 1;
        public int spawnChance = 100;

        public List<ChestItem> chestItems = new();

        public void OnValidate()
        {
            foreach (var chestItem in chestItems)
            {
                chestItem.tier = tier;
            }
        }
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(ChestItems))]
    public class ChestItemsEditor : Editor
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

            var config = target as ChestItems;

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