#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

public class BuildProfileSwitcher : MonoBehaviour
{
    private BuildProfile[] availableProfiles;

    public BuildProfile CurrentBuildProfile { get; private set; }

    private void RefreshProfiles()
    {
        availableProfiles = FetchAllBuildProfiles();
        CurrentBuildProfile = BuildProfile.GetActiveBuildProfile();
    }

    private BuildProfile[] FetchAllBuildProfiles()
    {
        string[] guids = AssetDatabase.FindAssets("t:BuildProfile");
        BuildProfile[] profiles = new BuildProfile[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            profiles[i] = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
        }

        return profiles;
    }

    public void SwitchToProfile(string profileName)
    {
        if (availableProfiles == null || availableProfiles.Length == 0)
        {
            RefreshProfiles();
        }

        foreach (var profile in availableProfiles)
        {
            if (profile.name == profileName)
            {
                BuildProfile.SetActiveBuildProfile(profile);
                Debug.Log($"Switched to build profile: {profileName}");
                return;
            }
        }

        Debug.LogError($"Build profile '{profileName}' not found!");
    }

    private void OnValidate()
    {
        RefreshProfiles();
    }
}
#endif