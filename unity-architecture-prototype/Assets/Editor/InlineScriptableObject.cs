using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineScriptableObject : PropertyDrawer
    {
        private Editor editor;
        private bool expand;

        private bool? hasEditorCache = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return base.GetPropertyHeight(property, label);

            if (!(property.objectReferenceValue is ScriptableObject) || HasEditor(property.serializedObject.targetObject))
                return base.GetPropertyHeight(property, label);

            return base.GetPropertyHeight(property, label) * 4f;

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 16f), property);

            if (property.objectReferenceValue == null)
                return;

            if (!(property.objectReferenceValue is ScriptableObject))
                return;

            if (!HasEditor(property.serializedObject.targetObject))
            {
                EditorGUI.HelpBox(new Rect(position.x, position.y + 18f, position.width, 32f), "Editing the ScriptableObject inline is not possible because no editor stub was found.", MessageType.Warning);
                if (GUI.Button(new Rect(position.x, position.y + 48f, position.width, 16f), "Create one for me!"))
                {
                    CreateStub(property.serializedObject.targetObject.GetType());
                    RefreshCache(property.serializedObject.targetObject);
                    Selection.activeObject = null;
                }
                return;
            }

            expand = EditorGUI.Foldout(new Rect(position.x, position.y, position.x + 10f, position.height), expand, "");

            if (!expand)
                return;

            EditorGUI.indentLevel++;
            property.serializedObject.Update();
            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            editor.OnInspectorGUI();
            property.serializedObject.ApplyModifiedProperties();
        }

        public bool HasEditor(Object obj)
        {
            if (hasEditorCache.HasValue)
                return hasEditorCache.Value;

            var hasEditor = RefreshCache(obj);
            return hasEditor;
        }

        private bool RefreshCache(Object obj)
        {
            var customEditors = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(type => type.IsSubclassOf(typeof(Editor)))
                .Select(type =>
                {
                    var attr = type.GetCustomAttributes(typeof(CustomEditor), true);
                    if (attr.Length <= 0)
                        return null;
                    return (CustomEditor)attr[0];
                });

            var hasEditor = false;
            foreach (var ed in customEditors)
            {
                if (ed == null) continue;

                var type = ed.GetType();
                var field = type.GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    var value = (Type)field.GetValue(ed);
                    if (value.IsInstanceOfType(obj))
                    {
                        hasEditor = true;
                        break;
                    }
                }
            }
            hasEditorCache = hasEditor;
            return hasEditor;
        }

        public void CreateStub(Type type)
        {
            string editorPath = "Assets/Editor";
            string[] assets = AssetDatabase.FindAssets("EditorStubs t:MonoScript");
            string stubFilePath = null;
            if (assets.Length <= 0)
            {
                if (!Directory.Exists(editorPath))
                {
                    Directory.CreateDirectory(editorPath);
                }
                stubFilePath = editorPath + "/EditorStubs.cs";
                File.WriteAllText(stubFilePath, "//This is a generated file - do not modify.\r\n");
                Debug.Log("Stub file created: " + stubFilePath);
            }

            if (string.IsNullOrEmpty(stubFilePath))
            {
                stubFilePath = AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            string customEditorAttribute = "[UnityEditor.CustomEditor(typeof(" + type.FullName + "))]";

            if (File.ReadAllLines(stubFilePath).Any(line => line.Contains(customEditorAttribute)))
            {
                Debug.LogError("Could not create editor stub since the stub file already contains such a stub.");
                return;
            }

            using (StreamWriter outfile = new StreamWriter(stubFilePath, true))
            {
                outfile.WriteLine(customEditorAttribute);
                outfile.WriteLine("public partial class " + type.Name + "EditorStub : UnityEditor.Editor { }");
                outfile.WriteLine("");
            }

            AssetDatabase.Refresh();
            Debug.Log("Wrote to stub file for type " + type.FullName);
        }
    }
