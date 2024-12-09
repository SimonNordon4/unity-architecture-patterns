using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FolderArchitectureReport : MonoBehaviour
{
    [Header("Folder Settings")]
    [Tooltip("The folder path relative to Assets (e.g., 'Assets/Scripts')")]
    public string folderPath = "Assets/";

    [HideInInspector]public int numberOfCSharpScripts;
    [HideInInspector]public int numberOfLinesOfCode;
    [HideInInspector]public int numberOfScriptableObjects;
    [HideInInspector]public float averageLinesOfCode;
    [HideInInspector]public float medianLinesOfCode;

    [HideInInspector]
    public List<int> linesOfCodePerScript = new List<int>();

    [HideInInspector]
    public List<string> scriptNamesPerScript = new List<string>();

    public void GenerateReport()
    {
        // Reset previous report data
        numberOfCSharpScripts = 0;
        numberOfLinesOfCode = 0;
        numberOfScriptableObjects = 0;
        averageLinesOfCode = 0f;
        medianLinesOfCode = 0f;
        linesOfCodePerScript.Clear();
        scriptNamesPerScript.Clear();

        // Validate folder
        if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Invalid folder path. Please specify a valid folder within the Assets directory.");
            return;
        }

        // Get C# scripts
        string absolutePath = Path.Combine(Application.dataPath, folderPath.Substring("Assets/".Length));
        string[] csFiles = Directory.GetFiles(absolutePath, "*.cs", SearchOption.AllDirectories);
        numberOfCSharpScripts = csFiles.Length;

        foreach (string file in csFiles)
        {
            try
            {
                int lineCount = File.ReadAllLines(file).Length;
                numberOfLinesOfCode += lineCount;
                linesOfCodePerScript.Add(lineCount);

                string relativePath = "Assets/" + Path.GetRelativePath(Application.dataPath, file).Replace("\\", "/");
                scriptNamesPerScript.Add(Path.GetFileName(relativePath));
            }
            catch
            {
                Debug.LogWarning($"Failed to read file: {file}");
            }
        }

        // Average and median calculations
        if (numberOfCSharpScripts > 0)
        {
            averageLinesOfCode = (float)numberOfLinesOfCode / numberOfCSharpScripts;

            var sortedLines = linesOfCodePerScript.OrderBy(n => n).ToList();
            int middle = sortedLines.Count / 2;
            medianLinesOfCode = sortedLines.Count % 2 == 0
                ? (sortedLines[middle - 1] + sortedLines[middle]) / 2f
                : sortedLines[middle];
        }

        // Find ScriptableObjects
        string[] scriptableObjectGUIDs = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });
        numberOfScriptableObjects = scriptableObjectGUIDs.Length;

        Debug.Log($"Report generated for {folderPath}. Scripts: {numberOfCSharpScripts}, ScriptableObjects: {numberOfScriptableObjects}");
    }
}
