using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(SoapSettings))]
    public class SoapSettingsDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Modify settings from the Soap Window:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Window/Obvious/Soap/Soap Window -> Settings");
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
        }
    }
}
