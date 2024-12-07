using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AutoGUIDReplacer : EditorWindow
{
    private string oldProjectPath = "Assets/OldProject";
    private string newProjectPath = "Assets/NewProject";

    [MenuItem("Tools/Auto GUID Replacer")]
    public static void ShowWindow()
    {
        GetWindow<AutoGUIDReplacer>("Auto GUID Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto GUID Replacer Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        oldProjectPath = EditorGUILayout.TextField("Old Project Path:", oldProjectPath);
        newProjectPath = EditorGUILayout.TextField("New Project Path:", newProjectPath);

        if (GUILayout.Button("Replace GUIDs Automatically"))
        {
            ReplaceGUIDsInProject();
        }
    }

    private void ReplaceGUIDsInProject()
    {
        if (!Directory.Exists(oldProjectPath) || !Directory.Exists(newProjectPath))
        {
            Debug.LogError("Invalid project paths. Please check the paths.");
            return;
        }

        // Step 1: Map old GUIDs to new GUIDs
        Dictionary<string, string> guidMap = BuildGUIDMap(oldProjectPath, newProjectPath);

        if (guidMap.Count == 0)
        {
            Debug.LogError("No GUIDs found to replace.");
            return;
        }

        // Step 2: Process all assets in the new project path
        string[] targetFiles = Directory.GetFiles(newProjectPath, "*.*", SearchOption.AllDirectories);

        foreach (var filePath in targetFiles)
        {
            if (filePath.EndsWith(".prefab") || filePath.EndsWith(".unity") || filePath.EndsWith(".asset"))
            {
                ReplaceGUIDsInFile(filePath, guidMap);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("GUID replacement complete for all scenes, prefabs, and ScriptableObjects in the new project.");
    }

    private Dictionary<string, string> BuildGUIDMap(string oldPath, string newPath)
    {
        Dictionary<string, string> guidMap = new Dictionary<string, string>();

        string[] oldMetaFiles = Directory.GetFiles(oldPath, "*.meta", SearchOption.AllDirectories);
        string[] newMetaFiles = Directory.GetFiles(newPath, "*.meta", SearchOption.AllDirectories);

        foreach (var oldMeta in oldMetaFiles)
        {
            string oldGUID = ExtractGUID(oldMeta);
            string newMeta = FindMatchingMeta(oldMeta, oldPath, newPath);

            if (!string.IsNullOrEmpty(newMeta))
            {
                string newGUID = ExtractGUID(newMeta);
                guidMap[oldGUID] = newGUID;
            }
        }

        return guidMap;
    }

    private void ReplaceGUIDsInFile(string filePath, Dictionary<string, string> guidMap)
    {
        string fileContent = File.ReadAllText(filePath);

        foreach (var kvp in guidMap)
        {
            string oldGUID = kvp.Key;
            string newGUID = kvp.Value;

            fileContent = fileContent.Replace(oldGUID, newGUID);
        }

        File.WriteAllText(filePath, fileContent);
        Debug.Log($"Processed file: {filePath}");
    }

    private string ExtractGUID(string metaFilePath)
    {
        foreach (var line in File.ReadAllLines(metaFilePath))
        {
            if (line.StartsWith("guid:"))
            {
                return line.Substring(5).Trim();
            }
        }
        return null;
    }

    private string FindMatchingMeta(string oldMetaPath, string oldBasePath, string newBasePath)
    {
        string relativePath = oldMetaPath.Replace(oldBasePath, "").Replace("\\", "/");
        string newMetaPath = Path.Combine(newBasePath, relativePath);

        return File.Exists(newMetaPath) ? newMetaPath : null;
    }
}
