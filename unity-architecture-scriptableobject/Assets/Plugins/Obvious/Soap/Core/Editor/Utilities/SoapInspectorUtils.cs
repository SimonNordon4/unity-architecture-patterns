using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    public static class SoapInspectorUtils
    {
        /// <summary>
        /// Draws all properties like base.OnInspectorGUI() but excludes the specified fields by name.
        /// </summary>
        /// <param name="fieldsToSkip">An array of names that should be excluded.</param>
        /// <example>Example: new string[] { "m_Script" , "myInt" } will skip the default Script field and the Integer field myInt.
        /// </example>
        public static void DrawInspectorExcept(this SerializedObject serializedObject, string[] fieldsToSkip)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (fieldsToSkip.Any(prop.name.Contains))
                        continue;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                } while (prop.NextVisible(false));
            }
        }
        
        public static void DrawCustomInspector(this SerializedObject serializedObject, string[] fieldsToSkip)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            var propertyCount = 0;
            
            if (prop.NextVisible(true))
            {
                do
                {
                    if (fieldsToSkip.Any(skipProp => prop.name == skipProp))
                        continue;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                    propertyCount++;

                    if (propertyCount != 4) 
                        continue;
                    
                    //Draw save properties
                    var isSaved = serializedObject.FindProperty("_saved");
                    if (!isSaved.boolValue)
                        continue;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    var guidProperty = serializedObject.FindProperty("_guid");
                    var saveGuid = serializedObject.FindProperty("_saveGuid");
                    var saveGuidType = (SaveGuidType)saveGuid.enumValueIndex;
                    EditorGUILayout.PropertyField(saveGuid, true);
                    if (saveGuidType == SaveGuidType.Auto)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(guidProperty.stringValue);
                        GUI.enabled = true;
                    }
                    else
                    {
                        guidProperty.stringValue = EditorGUILayout.TextField(guidProperty.stringValue);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                } while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary> Draw only the property specified. </summary>
        /// <param name="serializedObject"></param>
        /// <param name="fieldName"></param>
        public static void DrawOnlyField(this SerializedObject serializedObject, string fieldName, bool isReadOnly)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.name != fieldName)
                        continue;

                    GUI.enabled = !isReadOnly;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                    GUI.enabled = true;
                } while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Draws a line in the inspector. </summary>
        /// <param name="height"></param>
        public static void DrawLine(int height = 1)
        {
            DrawColoredLine(height, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        /// <summary> Draws a colored line in the inspector. </summary>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public static void DrawColoredLine(int height, Color color)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawSelectableObject(Object obj, string[] labels)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(labels[0], GUILayout.MaxWidth(300)))
                EditorGUIUtility.PingObject(obj);

            if (GUILayout.Button(labels[1], GUILayout.MaxWidth(75)))
            {
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        /// <summary> Creates a one Pixel texture.</summary>
        /// <param name="color"></param>
        public static Texture2D CreateTexture(Color color)
        {
            var result = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            result.SetPixel(0, 0, color);
            result.Apply();
            return result;
        }

        /// <summary> Centers a rect inside another window. </summary>
        /// <param name="window"></param>
        /// <param name="origin"></param>
        public static Rect CenterInWindow(Rect window, Rect origin)
        {
            var pos = window;
            float w = (origin.width - pos.width) * 0.5f;
            float h = (origin.height - pos.height) * 0.5f;
            pos.x = origin.x + w;
            pos.y = origin.y + h;
            return pos;
        }
    }
}