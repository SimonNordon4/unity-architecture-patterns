#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class VersionText : MonoBehaviour, IPreprocessBuildWithReport
{
    public int currentBuild = 7;
    
    public int callbackOrder { get; } = 0;

    private void OnEnable()
    {
        // This is just for convenience while in the editor
        // It doesn't increment, but formats and sets the text based on current value
        FormatAndSetVersionText();
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Incrementing build number");
        currentBuild++;
        FormatAndSetVersionText();
        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }

    private void FormatAndSetVersionText()
    {
        currentBuild = currentBuild % 1000;
        int major = currentBuild / 100;
        int minor = (currentBuild % 100) / 10;
        int patch = currentBuild % 10;
        GetComponent<TextMeshProUGUI>().text = $"v{major}.{minor}.{patch}";
    }
}
#endif