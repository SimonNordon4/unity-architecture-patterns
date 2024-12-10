#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

[CustomEditor(typeof(BuildProfileSwitcher))]
public class BuildProfileSwitcherEditor : Editor
{
    private BuildProfile[] availableProfiles;

    private void RefreshAvailableProfiles(BuildProfileSwitcher switcher)
    {
        if (availableProfiles == null || availableProfiles.Length == 0)
        {
            string[] guids = AssetDatabase.FindAssets("t:BuildProfile");
            availableProfiles = new BuildProfile[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                availableProfiles[i] = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
            }

            if (availableProfiles.Length == 0)
            {
                Debug.LogWarning("No Build Profiles found in the project.");
            }
        }
    }

    public override void OnInspectorGUI()
    {
        // Call base inspector GUI
        base.OnInspectorGUI();

        // Get the target object
        var switcher = (BuildProfileSwitcher)target;

        // Refresh profiles
        RefreshAvailableProfiles(switcher);

        // Display current build profile
        EditorGUILayout.LabelField("Current Build Profile:", 
            switcher.CurrentBuildProfile != null ? switcher.CurrentBuildProfile.name : "None");

        // Display a dropdown for all available profiles
        if (availableProfiles != null && availableProfiles.Length > 0)
        {
            foreach (var profile in availableProfiles)
            {
                if (GUILayout.Button($"Switch to {profile.name}"))
                {
                    BuildProfile.SetActiveBuildProfile(profile);
                    Debug.Log($"Switched to Build Profile: {profile.name}");
                    break;
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Build Profiles found. Please create and assign Build Profiles.", MessageType.Warning);
        }

        // Force repaint to ensure changes are reflected
        Repaint();
    }
}
#endif
