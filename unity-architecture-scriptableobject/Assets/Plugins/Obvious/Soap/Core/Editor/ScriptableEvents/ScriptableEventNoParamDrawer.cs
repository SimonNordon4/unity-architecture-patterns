using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableEventNoParam))]
    public class ScriptableEventNoParamDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Raise"))
            {
                var eventNoParam = (ScriptableEventNoParam)target;
                eventNoParam.Raise();
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
    }
}