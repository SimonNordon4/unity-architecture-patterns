using Classic.Character;
using UnityEditor;
using UnityEngine;

namespace Classic.Editor
{
    [CustomEditor(typeof(CharacterHealth))]
    public class CharacterHealthEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var health = (CharacterHealth) target;
            EditorGUILayout.LabelField($"Current Health: {health.currentHealth}");

            if (GUILayout.Button("Take Damage"))
            {
                health.TakeDamage(1);
            }
        }
    }
}