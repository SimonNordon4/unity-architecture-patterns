using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildProfileSwitcher))]
public class BuildProfileSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object
        var switcher = (BuildProfileSwitcher)target;

        // Display current build profile
        EditorGUILayout.LabelField("Current Build Profile:", 
            switcher.CurrentBuildProfile != null ? switcher.CurrentBuildProfile.name : "None");

        // Button to switch to Spaghetti Profile
        if (GUILayout.Button("Switch to Spaghetti Profile"))
        {
            switcher.SwitchToSpaghettiProfile();
        }

        // Button to switch to GameObject-Component Profile
        if (GUILayout.Button("Switch to GameObject-Component Profile"))
        {
            switcher.SwitchToGameObjectProfile();
        }

        // Button to switch to ScriptableObject Profile
        if (GUILayout.Button("Switch to ScriptableObject Profile"))
        {
            switcher.SwitchToScriptableObjectProfile();
        }
    }
}