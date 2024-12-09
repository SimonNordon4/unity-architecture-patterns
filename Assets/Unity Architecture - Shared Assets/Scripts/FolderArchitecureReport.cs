
#if UNITY_EDITOR
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

    /// <summary>
    /// Generates the architecture report based on the specified folder.
    /// </summary>
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

        // Calculate average and median lines of code
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

        // Sort the scripts from least to most lines of code
        SortScriptsByLines();

        Debug.Log($"Report generated for {folderPath}. Scripts: {numberOfCSharpScripts}, ScriptableObjects: {numberOfScriptableObjects}");
    }

    /// <summary>
    /// Sorts the scripts and their corresponding lines of code in ascending order based on lines of code.
    /// </summary>
    private void SortScriptsByLines()
    {
        // Create a list of tuples pairing lines of code with script names
        var scriptsWithLines = linesOfCodePerScript
            .Select((lines, index) => new { Lines = lines, Name = scriptNamesPerScript[index] })
            .OrderBy(script => script.Lines)
            .ToList();

        // Clear existing lists
        linesOfCodePerScript.Clear();
        scriptNamesPerScript.Clear();

        // Populate the lists with sorted data
        foreach (var script in scriptsWithLines)
        {
            linesOfCodePerScript.Add(script.Lines);
            scriptNamesPerScript.Add(script.Name);
        }
    }
}
#endif