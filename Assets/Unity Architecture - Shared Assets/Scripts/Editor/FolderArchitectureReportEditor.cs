using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(FolderArchitectureReport))]
public class FolderArchitectureReportEditor : Editor
{
    private const int GraphHeight = 200;
    private const int GraphWidth = 400;
    private const int BarWidth = 12;

    public override void OnInspectorGUI()
    {
        FolderArchitectureReport report = (FolderArchitectureReport)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Report"))
        {
            report.GenerateReport();
            EditorUtility.SetDirty(report);
        }

        if (report.numberOfCSharpScripts > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Report Summary", EditorStyles.boldLabel);
            GUILayout.Label($"C# Scripts: {report.numberOfCSharpScripts}");
            GUILayout.Label($"Total Lines of Code: {report.numberOfLinesOfCode}");
            GUILayout.Label($"Average Lines of Code per Script: {report.averageLinesOfCode:F2}");
            GUILayout.Label($"Median Lines of Code per Script: {report.medianLinesOfCode:F2}");
            GUILayout.Label($"ScriptableObjects: {report.numberOfScriptableObjects}");

            GUILayout.Space(10);
            GUILayout.Label("Lines of Code per Script", EditorStyles.boldLabel);
            DrawGraph(report);
        }
    }

    private void DrawGraph(FolderArchitectureReport report)
    {
        if (report.linesOfCodePerScript.Count == 0)
        {
            GUILayout.Label("No data to display graph.");
            return;
        }

        Rect graphArea = GUILayoutUtility.GetRect(GraphWidth, GraphHeight);
        EditorGUI.DrawRect(graphArea, new Color(0.18f, 0.18f, 0.18f)); // Graph background color

        int maxLines = report.linesOfCodePerScript.Max();
        float scale = (float)(GraphHeight - 20) / maxLines; // Leave padding

        for (int i = 0; i < report.linesOfCodePerScript.Count; i++)
        {
            int lines = report.linesOfCodePerScript[i];
            string scriptName = report.scriptNamesPerScript[i];

            float barHeight = lines * scale;
            float x = graphArea.x + i * BarWidth;
            float y = graphArea.y + GraphHeight - barHeight - 10;

            Rect barRect = new Rect(x, y, BarWidth - 1, barHeight); // -1 for spacing
            EditorGUI.DrawRect(barRect, new Color(0.0f, 0.9f, 0.0f)); // Green bar

            // Tooltip on hover
            if (Event.current.type == EventType.Repaint && barRect.Contains(Event.current.mousePosition))
            {
                GUIStyle tooltipStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
                Vector2 tooltipSize = tooltipStyle.CalcSize(new GUIContent($"{scriptName}\n{lines} LOC"));
                Rect tooltipRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - tooltipSize.y, tooltipSize.x, tooltipSize.y);
                GUI.Label(tooltipRect, $"{scriptName}\n{lines} LOC", tooltipStyle);
            }
        }
    }
}
