using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace UnityArchitecture.Meta
{
    public class GenerateArchitectureReport : EditorWindow
    {
        // Path of the selected folder
        private string selectedFolderPath = "Assets/";

        // Report Data
        private int numberOfCSharpScripts = 0;
        private int numberOfLinesOfCode = 0;
        private int numberOfScriptableObjects = 0;

        // Scroll position for the report display
        private Vector2 scrollPos;

        // Add menu item to open the window
        [MenuItem("Tools/Architecture Report")]
        public static void ShowWindow()
        {
            // Get existing open window or create a new one
            GetWindow<GenerateArchitectureReport>("Architecture Report");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate Architecture Report", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Folder Selection
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Folder:", GUILayout.Width(100));
            selectedFolderPath = EditorGUILayout.TextField(selectedFolderPath);

            if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Folder for Report", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    // Convert absolute path to relative path
                    if (path.StartsWith(Application.dataPath))
                    {
                        selectedFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder within the Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Generate Report Button
            if (GUILayout.Button("Generate Report", GUILayout.Height(40)))
            {
                GenerateReport();
            }

            GUILayout.Space(20);

            // Display Report
            if (numberOfCSharpScripts > 0 || numberOfLinesOfCode > 0 || numberOfScriptableObjects > 0)
            {
                GUILayout.Label("Report:", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));

                GUILayout.Label($"Number of C# Scripts: {numberOfCSharpScripts}");
                GUILayout.Label($"Number of Lines of Code: {numberOfLinesOfCode}");
                GUILayout.Label($"Number of ScriptableObjects: {numberOfScriptableObjects}");

                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Generates the architecture report based on the selected folder.
        /// </summary>
        private void GenerateReport()
        {
            // Reset previous report data
            numberOfCSharpScripts = 0;
            numberOfLinesOfCode = 0;
            numberOfScriptableObjects = 0;

            // Validate the selected folder
            if (string.IsNullOrEmpty(selectedFolderPath) || !AssetDatabase.IsValidFolder(selectedFolderPath))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Please select a valid folder within the Assets directory.", "OK");
                return;
            }

            // Get absolute path from relative path
            string absolutePath = Path.Combine(Application.dataPath, selectedFolderPath.Substring("Assets/".Length));

            // Ensure the path exists
            if (!Directory.Exists(absolutePath))
            {
                EditorUtility.DisplayDialog("Folder Not Found", $"The folder '{selectedFolderPath}' does not exist.", "OK");
                return;
            }

            // Get all .cs files in the folder and subfolders
            string[] csFiles = Directory.GetFiles(absolutePath, "*.cs", SearchOption.AllDirectories);
            numberOfCSharpScripts = csFiles.Length;

            // Count total lines of code
            foreach (string file in csFiles)
            {
                try
                {
                    int lineCount = File.ReadAllLines(file).Length;
                    numberOfLinesOfCode += lineCount;
                }
                catch
                {
                    Debug.LogWarning($"Failed to read file: {file}");
                }
            }

            // Find all ScriptableObject assets in the folder and subfolders
            string[] allAssetPaths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { selectedFolderPath })
                                               .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                                               .ToArray();

            numberOfScriptableObjects = allAssetPaths.Length;

            // Optionally, you can further filter or process the ScriptableObjects here

            // Refresh the window to display updated report
            Repaint();
        }
    }
}
