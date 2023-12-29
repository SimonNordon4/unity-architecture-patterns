using GameObjectComponent.Game;
using UnityEditor;
using UnityEngine;

namespace GameObjectComponent.Editor
{
    [CustomEditor(typeof(Gold))]
    public class GoldEditor : UnityEditor.Editor
    {
        private int goldToAdd = 1;
        public override void OnInspectorGUI()
        {
            var gold = (Gold) target;
            base.OnInspectorGUI();

            // create a text field that takes a number, and a button next to it to add gold
            goldToAdd = EditorGUILayout.IntField("Gold to add", goldToAdd);
            if (GUILayout.Button("Add Gold"))
            {
                gold.AddGold(goldToAdd);
            }
        }        
    }
}