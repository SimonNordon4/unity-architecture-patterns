using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor
{
    public static class SoapEditorUtils
    {
        [MenuItem("Tools/Obvious/Soap/Delete Player Pref %#d", priority = 0)]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log($"<color={SoapColorHtml}>--Player Prefs deleted--</color>");
        }

        [MenuItem("Tools/Obvious/Soap/ToggleFastPlayMode %l", priority = 1)]
        public static void ToggleFastPlayMode()
        {
            EditorSettings.enterPlayModeOptionsEnabled = !EditorSettings.enterPlayModeOptionsEnabled;
            AssetDatabase.Refresh();
            var text = EditorSettings.enterPlayModeOptionsEnabled
                ? "<color=#55efc4>Enabled"
                : $"<color={SoapColorHtml}>Disabled";
            text += "</color>";
            Debug.Log("Fast Play Mode " + text);
        }

        [MenuItem("CONTEXT/ScriptableBase/Reset")]
        private static void Reset(MenuCommand command) => ResetToDefaultValue(command.context);

        [ContextMenu("Reset To Default Value")]
        private static void ResetToDefaultValue(Object unityObject)
        {
            var reset = unityObject as ScriptableBase;
            reset.Reset();
            EditorUtility.SetDirty(unityObject);
        }

        [MenuItem("CONTEXT/ScriptableVariableBase/ResetToInitialValue")]
        private static void ResetToInitialValue(MenuCommand command) => ResetToInitialValue(command.context);

        [ContextMenu("Reset To Initial Values")]
        private static void ResetToInitialValue(Object unityObject)
        {
            var reset = unityObject as IReset;
            reset.ResetToInitialValue();
        }

        public static Color SoapColor => ColorUtility.TryParseHtmlString(SoapColorHtml, out var color) ? color : Color.magenta;
        
        private const string SoapColorHtml = "#f75369";
        
        public static Color Lighten(this Color color, float amount)
        {
            return new Color(color.r + amount, color.g + amount, color.b + amount, color.a);
        }

        /// <summary>
        /// Returns or creates the SoapSettings ScriptableObject located in the Soap Core resources folder.
        /// </summary>
        public static SoapSettings GetOrCreateSoapSettings()
        {
            var settings = Resources.Load<SoapSettings>("SoapSettings");
            if (settings != null)
                return settings;

            var paths = SoapFileUtils.GetResourcesDirectories();
            foreach (var path in paths)
            {
                var relative = SoapFileUtils.GetRelativePath(path);
                if (!relative.Contains("Obvious") || !relative.Contains("Editor"))
                    continue;
                var finalPath = relative + "/SoapSettings.asset";
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(SoapSettings)), finalPath);
            }

            return settings;
        }

        /// <summary>
        /// Deletes an object after showing a confirmation dialog.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns> true if the deletion is confirmed</returns>
        public static bool DeleteObjectWithConfirmation(Object obj)
        {
            var confirmDelete = EditorUtility.DisplayDialog("Delete " + obj.name + "?",
                "Are you sure you want to delete '" + obj.name + "'?", "Yes", "No");
            if (confirmDelete)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.DeleteAsset(path);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a copy of an object. Also adds a number to the copy name.
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateCopy(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var copyFilePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CopyAsset(path, copyFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var newAsset = AssetDatabase.LoadMainAssetAtPath(copyFilePath);
            EditorGUIUtility.PingObject(newAsset);
        }

        /// <summary> Creates a Soap Scriptable object at a certain path </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        public static ScriptableObject CreateScriptableObjectAt(Type type, string name, string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var endPath = path.Replace("Assets/", "");
                AssetDatabase.CreateFolder("Assets", endPath);
            }

            var instance = ScriptableObject.CreateInstance(type);
            instance.name = name == "" ? type.ToString().Replace("Obvious.Soap.", "") : name;
            var creationPath = $"{path}/{instance.name}.asset";
            var uniqueFilePath = AssetDatabase.GenerateUniqueAssetPath(creationPath);
            var isAuto = GetOrCreateSoapSettings().NamingOnCreationMode == ENamingCreationMode.Auto;
            if (isAuto)
            {
                AssetDatabase.CreateAsset(instance, uniqueFilePath);
                EditorGUIUtility.PingObject(instance);
            }
            else
            {
                ProjectWindowUtil.CreateAsset(instance, uniqueFilePath);
            }
            return instance;
        }
        
        /// <summary> Renames an asset </summary>
        /// <param name="obj"></param
        /// <param name="newName"></param>
        public static void RenameAsset(Object obj, string newName)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(obj);
        }

        /// <summary>
        /// Creates a new class from a template.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <param name="newFile"></param>
        /// <param name="isSoapClass"></param>
        /// <returns></returns>
        public static bool CreateClassFromTemplate(string template, string type, string path,
            out TextAsset newFile, bool isSoapClass = false)
        {
            var folderName = "Templates/";
            folderName += template;
            var capitalizedName = type.CapitalizeFirstLetter();
            
            var fileName = capitalizedName+".cs";
            if (isSoapClass)
            {
                fileName = template.Contains("Variable")
                    ? $"{capitalizedName}Variable.cs"
                    : template.Replace("Template", capitalizedName);
            }
            
            newFile = CreateNewClass(folderName, type, fileName, path);
            return newFile != null;
        }

        private static TextAsset CreateNewClass(string templateName, string type, string fileName, string path)
        {
            if (!SoapTypeUtils.IsTypeNameValid(type))
                return null;

            var template = Resources.Load<TextAsset>(templateName);
            if (template is null)
            {
                Debug.LogError($"Failed to find {templateName} in a Resources folder");
                return null;
            }

            var templateCode = template.text;
            templateCode = templateCode.Replace("#TYPE#", type);
            templateCode = templateCode.Replace("#TYPENAME#", type.CapitalizeFirstLetter());
            try
            {
                var newFile = CreateTextFile(templateCode, fileName, path);
                return newFile;
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("Could not create class", e.Message, "OK");
                return null;
            }
        }


        /// <summary> Creates a new text asset at a certain location. </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <exception cref="IOException"></exception>
        public static TextAsset CreateTextFile(string content, string fileName, string path)
        {
            var filePath = path + "/" + fileName;
            var folderPath = Directory.GetParent(Application.dataPath).FullName + "/" + path;
            var fullPath = folderPath + "/" + fileName;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (File.Exists(fullPath))
                throw new IOException($"A file with the name {filePath} already exists.");

            File.WriteAllText(fullPath, content);
            AssetDatabase.Refresh();
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            return textAsset;
        }

        /// <summary> Returns all ScriptableObjects at a certain path. </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public static List<T> FindAll<T>(string path = "") where T : ScriptableObject
        {
            var scripts = new List<T>();
            var searchFilter = $"t:{typeof(T).Name}";
            var soNames = path == ""
                ? AssetDatabase.FindAssets(searchFilter)
                : AssetDatabase.FindAssets(searchFilter, new[] { path });

            foreach (var soName in soNames)
            {
                var soPath = AssetDatabase.GUIDToAssetPath(soName);
                var script = AssetDatabase.LoadAssetAtPath<T>(soPath);
                if (script == null)
                    continue;

                scripts.Add(script);
            }

            return scripts;
        }

        public static void DrawSerializationError(Type type, Rect position = default)
        {
            if (position == default)
            {
                EditorGUILayout.HelpBox($"{type} is not marked as Serializable," +
                                        "\n Add [System.Serializable] attribute.", MessageType.Error);
            }
            else
            {
                var icon = EditorGUIUtility.IconContent("Error").image;
                GUI.DrawTexture(position, icon, ScaleMode.ScaleToFit);
            }
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// Clear editor Prefs for Soap.
        /// </summary>
        public static void ClearEditorPrefs()
        {
            EditorPrefs.DeleteKey(SoapWizardWindow.PathKey);
            EditorPrefs.DeleteKey(SoapWizardWindow.FavoriteKey);
            EditorPrefs.DeleteKey(SoapWizardWindow.CategoriesKey);
            EditorPrefs.DeleteKey(SoapWizardWindow.CategoriesLayoutKey);
            EditorPrefs.DeleteKey(SoapWindow.LastCategoryKey);
            EditorPrefs.DeleteKey(SoapWindowInitializer.HasShownWindowKey);
            Debug.Log("Editor prefs deleted");
        }

        /// <summary>
        /// Set the Game View Window to the first preset "Free Aspect".
        /// </summary>
        public static void SetGameViewScaleAndSize()
        {
            var assembly = typeof(EditorWindow).Assembly;
            var gameViewType = assembly.GetType("UnityEditor.GameView");
            var gameViewWindow = EditorWindow.GetWindow(gameViewType);

            if (gameViewWindow == null)
            {
                Debug.LogError("GameView is null!");
                return;
            }

            var resolutionField = gameViewType.GetProperty("lowResolutionForAspectRatios");
            resolutionField.SetValue(gameViewWindow, false);
            float defaultScale = 1f;
            var areaField = gameViewType.GetField("m_ZoomArea",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var areaObj = areaField.GetValue(gameViewWindow);
            var scaleField = areaObj.GetType().GetField("m_Scale",
                BindingFlags.Instance | BindingFlags.NonPublic);
            scaleField.SetValue(areaObj, new Vector2(defaultScale, defaultScale));

            var selectedSizeIndexProp = gameViewType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            selectedSizeIndexProp.SetValue(gameViewWindow, 0, null);
        }
    }
}