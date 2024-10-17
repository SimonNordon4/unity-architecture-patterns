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
        private float averageLinesOfCode = 0f;
        private float medianLinesOfCode = 0f;
        private List<int> linesOfCodePerScript = new List<int>(); // Stores lines of code for each script

        // Scroll position for the report display
        private Vector2 scrollPos;

        // Graph parameters
        private const int graphHeight = 200;
        private const int graphWidth = 400;
        private const int barWidth = 5;
        private Texture2D graphTexture;

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
                GUILayout.Label($"Average Lines of Code per Script: {averageLinesOfCode:F2}");
                GUILayout.Label($"Median Lines of Code per Script: {medianLinesOfCode:F2}");

                EditorGUILayout.EndScrollView();

                GUILayout.Space(20);

                // Display Graph
                GUILayout.Label("Lines of Code per Script:", EditorStyles.boldLabel);
                if (graphTexture != null)
                {
                    // Ensure the graph fits within the window
                    GUILayout.Label(graphTexture, GUILayout.Width(graphWidth), GUILayout.Height(graphHeight));
                }
                else
                {
                    GUILayout.Label("No data to display graph.");
                }
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
            averageLinesOfCode = 0f;
            medianLinesOfCode = 0f;
            linesOfCodePerScript.Clear();
            graphTexture = null;

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

            // Count total lines of code and collect lines per script
            foreach (string file in csFiles)
            {
                try
                {
                    int lineCount = File.ReadAllLines(file).Length;
                    numberOfLinesOfCode += lineCount;
                    linesOfCodePerScript.Add(lineCount);
                }
                catch
                {
                    Debug.LogWarning($"Failed to read file: {file}");
                }
            }

            // Calculate average lines of code
            if (numberOfCSharpScripts > 0)
            {
                averageLinesOfCode = (float)numberOfLinesOfCode / numberOfCSharpScripts;
            }

            // Calculate median lines of code
            if (linesOfCodePerScript.Count > 0)
            {
                var sortedLines = linesOfCodePerScript.OrderBy(n => n).ToList();
                int middle = sortedLines.Count / 2;
                if (sortedLines.Count % 2 == 0)
                {
                    // Even number of elements
                    medianLinesOfCode = (sortedLines[middle - 1] + sortedLines[middle]) / 2f;
                }
                else
                {
                    // Odd number of elements
                    medianLinesOfCode = sortedLines[middle];
                }
            }

            // Find all ScriptableObject assets in the folder and subfolders
            string[] allAssetPaths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { selectedFolderPath })
                                               .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                                               .ToArray();

            numberOfScriptableObjects = allAssetPaths.Length;

            // Generate Graph
            GenerateGraph();

            // Refresh the window to display updated report
            Repaint();
        }

        /// <summary>
        /// Generates a simple bar graph texture showing lines of code per script.
        /// </summary>
        private void GenerateGraph()
        {
            if (linesOfCodePerScript.Count == 0)
                return;

            // Define graph dimensions
            int width = graphWidth;
            int height = graphHeight;

            // Create a new texture
            graphTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color backgroundColor = new Color(0.18f,0.18f, 0.18f);
            Color barColor = new Color(0.0f, 0.9f, 0.0f);
            // Initialize texture with white background
            Color[] bgPixels = Enumerable.Repeat(backgroundColor, width * height).ToArray();
            graphTexture.SetPixels(bgPixels);

            // Determine the maximum lines of code to scale the graph
            int maxLines = linesOfCodePerScript.Max();

            // Calculate scaling factor
            float scale = (float)(height - 20) / maxLines; // Leave some padding

            // Draw bars
            for (int i = 0; i < linesOfCodePerScript.Count; i++)
            {
                int x = i * barWidth;
                if (x + barWidth >= width)
                    break; // Prevent drawing outside the texture

                int barHeightPixels = Mathf.RoundToInt(linesOfCodePerScript[i] * scale);
                for (int bx = x; bx < x + barWidth; bx++)
                {
                    for (int by = 0; by < barHeightPixels; by++)
                    {
                        if (bx >= width || by >= height)
                            continue;
                        graphTexture.SetPixel(bx, by + 10, barColor); // +10 for bottom padding
                    }
                }
            }

            graphTexture.Apply();
        }
    }
}
