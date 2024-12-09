using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(FolderArchitectureReport))]
public class FolderArchitectureReportEditor : Editor
{
    private const int GraphHeight = 200;
    private const int GraphWidth = 400;
    private const int BarWidth = 12;

    // Label to display the selected script
    private string selectedScript = "None";

    public override void OnInspectorGUI()
    {
        // Reference to the target script
        FolderArchitectureReport report = (FolderArchitectureReport)target;

        // Draw the default inspector (Folder Path and Report Data)
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Generate Report Button
        if (GUILayout.Button("Generate Report", GUILayout.Height(40)))
        {
            Undo.RecordObject(report, "Generate Architecture Report");
            report.GenerateReport();
            EditorUtility.SetDirty(report);
        }

        GUILayout.Space(20);

        // Display Report Summary
        if (report.numberOfCSharpScripts > 0 || report.numberOfLinesOfCode > 0 || report.numberOfScriptableObjects > 0)
        {
            GUILayout.Label("Report Summary", EditorStyles.boldLabel);
            GUILayout.Label($"C# Scripts: {report.numberOfCSharpScripts}");
            GUILayout.Label($"Total Lines of Code: {report.numberOfLinesOfCode}");
            GUILayout.Label($"Average Lines of Code per Script: {report.averageLinesOfCode:F2}");
            GUILayout.Label($"Median Lines of Code per Script: {report.medianLinesOfCode:F2}");
            GUILayout.Label($"ScriptableObjects: {report.numberOfScriptableObjects}");

            GUILayout.Space(20);

            // Display Graph
            GUILayout.Label("Lines of Code per Script", EditorStyles.boldLabel);
            DrawGraph(report);

            GUILayout.Space(10);

            // Display Selected Script
            GUILayout.Label($"Selected Script: {selectedScript}", EditorStyles.boldLabel);
        }
    }

    /// <summary>
    /// Draws a bar graph representing lines of code per script and updates the selected script label.
    /// </summary>
    private void DrawGraph(FolderArchitectureReport report)
    {
        if (report.linesOfCodePerScript.Count == 0)
        {
            GUILayout.Label("No data to display graph.");
            return;
        }

        // Define graph area
        Rect graphArea = GUILayoutUtility.GetRect(GraphWidth, GraphHeight);
        EditorGUI.DrawRect(graphArea, new Color(0.18f, 0.18f, 0.18f)); // Dark background

        // Determine the maximum lines of code to scale the graph
        int maxLines = report.linesOfCodePerScript.Max();
        if (maxLines == 0) maxLines = 1; // Prevent division by zero

        // Calculate scaling factor
        float scale = (float)(GraphHeight - 20) / maxLines; // Leave some padding

        // Iterate through each script to draw its bar
        for (int i = 0; i < report.linesOfCodePerScript.Count; i++)
        {
            int lines = report.linesOfCodePerScript[i];
            string scriptName = report.scriptNamesPerScript[i];

            // Calculate bar height
            float barHeightPixels = lines * scale;

            // Calculate position
            float x = graphArea.x + i * BarWidth;
            float y = graphArea.y + GraphHeight - barHeightPixels - 10; // 10 pixels padding at bottom

            // Define bar Rect
            Rect barRect = new Rect(x, y, BarWidth - 2, barHeightPixels); // -2 for spacing between bars

            // Draw the bar
            EditorGUI.DrawRect(barRect, new Color(0.0f, 0.6f, 1.0f)); // Blue bars

            // Check if mouse is over this bar
            if (barRect.Contains(Event.current.mousePosition))
            {
                selectedScript = $"{scriptName} ({lines} LOC)";
                // Repaint to ensure the label updates immediately
                Repaint();
                return; // Exit early since only one bar can be hovered at a time
            }
        }

        // If no bar is hovered, reset the selected script
        selectedScript = "None";
    }
}
