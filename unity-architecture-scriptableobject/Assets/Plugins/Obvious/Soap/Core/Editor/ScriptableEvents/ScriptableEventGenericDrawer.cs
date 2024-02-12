using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableEventBase), true)]
    public class ScriptableEventGenericDrawer : UnityEditor.Editor
    {
        private MethodInfo _methodInfo;
        private ScriptableEventBase _scriptableEventBase;

        private void OnEnable()
        {
            _methodInfo = target.GetType().BaseType.GetMethod("Raise",
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_scriptableEventBase == null)
                _scriptableEventBase = target as ScriptableEventBase;
            var genericType = _scriptableEventBase.GetGenericType;
            if (!SoapTypeUtils.IsSerializable(genericType))
            {
                SoapEditorUtils.DrawSerializationError(genericType);
                return;
            }

            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Raise"))
            {
                var property = serializedObject.FindProperty("_debugValue");
                _methodInfo.Invoke(target, new[] { GetDebugValue(property) });
            }

            GUI.enabled = true;

            if (!EditorApplication.isPlaying)
                return;

            SoapInspectorUtils.DrawLine();

            var goContainer = (IDrawObjectsInInspector)target;
            var gameObjects = goContainer.GetAllObjects();
            if (gameObjects.Count > 0)
                DisplayAll(gameObjects);
        }

        private void DisplayAll(List<Object> objects)
        {
            GUILayout.Space(15);
            var title = $"Listener Amount : {objects.Count}";

            GUILayout.BeginVertical(title, "window");
            foreach (var obj in objects)
                SoapInspectorUtils.DrawSelectableObject(obj, new[] { obj.name, "Select" });
            GUILayout.EndVertical();
        }

        private object GetDebugValue(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            var targetField = targetType.GetField("_debugValue", BindingFlags.Instance | BindingFlags.NonPublic);
            return targetField.GetValue(property.serializedObject.targetObject);
        }
    }
}