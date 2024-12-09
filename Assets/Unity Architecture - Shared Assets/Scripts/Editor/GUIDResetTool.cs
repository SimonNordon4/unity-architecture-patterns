using UnityEditor;
using UnityEngine;
using System.IO;

public class GUIDResetTool : EditorWindow
{
    private string selectedFolderPath = "";

    [MenuItem("Tools/GUID Reset Tool")]
    public static void ShowWindow()
    {
        var window = GetWindow<GUIDResetTool>("GUID Reset Tool");
        window.minSize = new Vector2(400, 200);
    }

    private void OnGUI()
    {
        GUILayout.Label("GUID Reset Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Select Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");

            if (!string.IsNullOrEmpty(folderPath))
            {
                selectedFolderPath = ConvertToProjectRelativePath(folderPath);
            }
        }

        EditorGUILayout.LabelField("Selected Folder:", selectedFolderPath);

        if (!string.IsNullOrEmpty(selectedFolderPath))
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset GUIDs in Selected Folder", GUILayout.Height(40)))
            {
                ResetGUIDsInFolder(selectedFolderPath);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a folder to reset GUIDs.", MessageType.Info);
        }
    }

    private void ResetGUIDsInFolder(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path. Please select a valid folder.");
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (file.EndsWith(".meta"))
            {
                File.Delete(file);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"All GUIDs in folder {folderPath} have been reset.");
    }

    private static string ConvertToProjectRelativePath(string absolutePath)
    {
        string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

        if (absolutePath.StartsWith(projectPath))
        {
            return "Assets" + absolutePath.Substring(projectPath.Length).Replace("\\", "/");
        }

        Debug.LogError("Selected folder is not within the Unity project.");
        return "";
    }
}
