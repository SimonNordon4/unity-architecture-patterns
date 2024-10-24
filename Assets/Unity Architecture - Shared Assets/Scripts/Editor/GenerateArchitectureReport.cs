using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace UnityArchitecture.SharedAssets
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
        private List<string> scriptNamesPerScript = new List<string>(); // Stores script names

        // Scroll position for the report display
        private Vector2 scrollPos;

        // Graph parameters
        private const int graphHeight = 200;
        private const int graphWidth = 400;
        private const int barWidth = 12;

        // Tooltip variables
        private string tooltipText = "";
        private Rect tooltipRect = new Rect();

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
                DrawGraph();
            }

            // Display Tooltip if any
            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Draw a semi-transparent box as background for the tooltip
                GUIStyle tooltipStyle = new GUIStyle(EditorStyles.helpBox);
                tooltipStyle.alignment = TextAnchor.UpperLeft;
                tooltipStyle.normal.textColor = Color.white;

                // Adjust tooltip position to stay within the window
                Rect adjustedRect = tooltipRect;
                if (adjustedRect.x + adjustedRect.width > position.width)
                {
                    adjustedRect.x = position.width - adjustedRect.width - 10;
                }
                if (adjustedRect.y + adjustedRect.height > position.height)
                {
                    adjustedRect.y = position.height - adjustedRect.height - 10;
                }

                GUI.Label(adjustedRect, tooltipText, tooltipStyle);
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
            scriptNamesPerScript.Clear();

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

                    // Store the script name (relative to selected folder)
                    string relativePath = "Assets/" + Path.GetRelativePath(Application.dataPath, file).Replace("\\", "/");
                    scriptNamesPerScript.Add(Path.GetFileName(relativePath));
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

            // Sort the scripts from least to most lines of code
            SortScriptsByLines();

            // Find all ScriptableObject assets in the folder and subfolders
            string[] allAssetPaths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { selectedFolderPath })
                                               .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                                               .ToArray();

            numberOfScriptableObjects = allAssetPaths.Length;

            // Refresh the window to display updated report
            Repaint();
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

        /// <summary>
        /// Draws a simple bar graph showing lines of code per script with interactive tooltips.
        /// The graph is sorted from least lines to most lines.
        /// </summary>
        private void DrawGraph()
        {
            if (linesOfCodePerScript.Count == 0 || scriptNamesPerScript.Count == 0)
            {
                GUILayout.Label("No data to display graph.");
                return;
            }

            // Define graph area
            Rect graphArea = GUILayoutUtility.GetRect(graphWidth, graphHeight);
            EditorGUI.DrawRect(graphArea, new Color(0.18f, 0.18f, 0.18f)); // Dark background

            // Determine the maximum lines of code to scale the graph
            int maxLines = linesOfCodePerScript.Max();

            // Calculate scaling factor
            float scale = (float)(graphHeight - 20) / maxLines; // Leave some padding

            // Reset tooltip variables
            tooltipText = "";
            tooltipRect = new Rect();

            // Iterate through each script to draw its bar
            for (int i = 0; i < linesOfCodePerScript.Count; i++)
            {
                int lines = linesOfCodePerScript[i];
                string scriptName = scriptNamesPerScript[i];

                // Calculate bar height
                int barHeightPixels = Mathf.RoundToInt(lines * scale);

                // Calculate position
                float x = graphArea.x + i * barWidth;
                float y = graphArea.y + graphHeight - barHeightPixels - 10; // 10 pixels padding at bottom

                // Define bar Rect
                Rect barRect = new Rect(x, y, barWidth - 1, barHeightPixels); // -1 for spacing between bars

                // Draw the bar
                EditorGUI.DrawRect(barRect, new Color(0.0f, 0.9f, 0.0f)); // Green bars

                // Check if mouse is over this bar
                if (barRect.Contains(Event.current.mousePosition))
                {
                    tooltipText = $"{scriptName}\nLines of Code: {lines}";
                    tooltipRect = new Rect(Event.current.mousePosition.x + 10, Event.current.mousePosition.y + 10, 200, 40);
                }

                // Optional: Draw script name below each bar (can be commented out if too cluttered)
                /*
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.fontSize = 8;
                labelStyle.alignment = TextAnchor.UpperCenter;
                Rect labelRect = new Rect(x, graphArea.y + graphHeight - 15, barWidth - 1, 15);
                GUI.Label(labelRect, scriptName, labelStyle);
                */
            }

            // Handle tooltip display
            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Ensure the tooltip does not go outside the window bounds
                Vector2 mousePos = Event.current.mousePosition;
                float tooltipWidth = 200;
                float tooltipHeight = 40;

                // Adjust tooltip position if it exceeds window bounds
                if (tooltipRect.x + tooltipWidth > position.width)
                {
                    tooltipRect.x = position.width - tooltipWidth - 10;
                }
                if (tooltipRect.y + tooltipHeight > position.height)
                {
                    tooltipRect.y = position.height - tooltipHeight - 10;
                }

                // Draw the tooltip background
                GUIStyle tooltipStyle = new GUIStyle(EditorStyles.helpBox);
                tooltipStyle.normal.textColor = Color.white;
                tooltipStyle.alignment = TextAnchor.UpperLeft;

                GUI.Label(new Rect(tooltipRect.x, tooltipRect.y, tooltipWidth, tooltipHeight), tooltipText, tooltipStyle);
            }
        }
    }
}
