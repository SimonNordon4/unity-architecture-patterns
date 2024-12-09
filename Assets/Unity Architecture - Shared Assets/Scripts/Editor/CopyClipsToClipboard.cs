#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityArchitecture.SharedAssets
{
    public static class CopyScriptsToClipboard
    {
        /// <summary>
        /// Adds a context menu item "Copy Scripts to Clipboard" when right-clicking on a folder in the Project window.
        /// </summary>
        [MenuItem("Assets/Copy Scripts to Clipboard", false, 2000)]
        private static void CopyScripts()
        {
            // Get the selected asset path
            string selectedPath = GetSelectedFolderPath();

            if (string.IsNullOrEmpty(selectedPath))
            {
                EditorUtility.DisplayDialog("Copy Scripts to Clipboard", "Please select a valid folder in the Project window.", "OK");
                return;
            }

            // Get all .cs files in the selected folder and its subfolders
            string[] csFiles = Directory.GetFiles(selectedPath, "*.cs", SearchOption.AllDirectories);

            if (csFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Copy Scripts to Clipboard", "No C# scripts found in the selected folder and its subfolders.", "OK");
                return;
            }

            // Initialize a StringBuilder for efficient concatenation
            System.Text.StringBuilder combinedScripts = new System.Text.StringBuilder();

            foreach (string filePath in csFiles)
            {
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    combinedScripts.AppendLine($"// ----- {Path.GetFileName(filePath)} -----");
                    combinedScripts.AppendLine(fileContent);
                    combinedScripts.AppendLine(); // Add an empty line between scripts for readability
                }
                catch (IOException ex)
                {
                    Debug.LogWarning($"Failed to read file: {filePath}\nException: {ex.Message}");
                }
            }

            // Copy the combined scripts to the system clipboard
            EditorGUIUtility.systemCopyBuffer = combinedScripts.ToString();

            // Inform the user of the successful operation
            EditorUtility.DisplayDialog("Copy Scripts to Clipboard", $"Successfully copied {csFiles.Length} scripts to the clipboard.", "OK");
        }

        /// <summary>
        /// Validates whether the context menu item should be enabled.
        /// The menu item is only enabled when a single folder is selected.
        /// </summary>
        /// <returns>True if a single folder is selected; otherwise, false.</returns>
        [MenuItem("Assets/Copy Scripts to Clipboard", true)]
        private static bool ValidateCopyScripts()
        {
            // Ensure that exactly one object is selected
            if (Selection.objects.Length != 1)
                return false;

            // Get the selected object
            Object selectedObject = Selection.activeObject;

            // Get the asset path
            string path = AssetDatabase.GetAssetPath(selectedObject);

            // Check if the selected path is a valid folder
            return AssetDatabase.IsValidFolder(path);
        }

        /// <summary>
        /// Retrieves the absolute file system path of the selected folder in the Project window.
        /// </summary>
        /// <returns>The absolute path of the selected folder, or null if not found.</returns>
        private static string GetSelectedFolderPath()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject == null)
                return null;

            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                // Convert to absolute path
                string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
                return absolutePath;
            }

            return null;
        }
    }
}
#endif