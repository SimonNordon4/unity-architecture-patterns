#if UNITY_EDITOR
using UnityEditor.Build.Profile;

using UnityEngine;


public class BuildProfileSwitcher : MonoBehaviour
{

    [SerializeField]private BuildProfile spaghettiProfile;
    [SerializeField]private BuildProfile gameObjectComponentProfile;
    [SerializeField]private BuildProfile scriptableObjectProfile;
    
    public BuildProfile CurrentBuildProfile { get; private set; }

    public void RefreshProfiles()
    {
        CurrentBuildProfile = BuildProfile.GetActiveBuildProfile();
    }

    public void SwitchToSpaghettiProfile()
    {
        BuildProfile.SetActiveBuildProfile(CurrentBuildProfile);
    }

    public void SwitchToGameObjectProfile()
    {
        BuildProfile.SetActiveBuildProfile(gameObjectComponentProfile);
    }

    public void SwitchToScriptableObjectProfile()
    {
        BuildProfile.SetActiveBuildProfile(scriptableObjectProfile);
    }

    private void OnValidate()
    {
        RefreshProfiles();
    }

}
#endif
