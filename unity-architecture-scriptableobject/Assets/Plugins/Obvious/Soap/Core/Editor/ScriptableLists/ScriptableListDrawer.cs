using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableListBase), true)]
    public class ScriptableListDrawer : UnityEditor.Editor
    {
        private ScriptableListBase _scriptableListBase;
        private ScriptableBase _scriptableBase;
        private static bool _repaintFlag;

        public override void OnInspectorGUI()
        {
            if (_scriptableListBase == null)
                _scriptableListBase = target as ScriptableListBase;

            var isMonoBehaviourOrGameObject = _scriptableListBase.GetGenericType.IsSubclassOf(typeof(MonoBehaviour))
                                              || _scriptableListBase.GetGenericType == typeof(GameObject);
            if (isMonoBehaviourOrGameObject)
            {
                serializedObject.DrawOnlyField("m_Script", true);
                serializedObject.DrawOnlyField("_resetOn", false);
            }
            else
            {
                //we still want to display the native list for non MonoBehaviors (like SO for examples)
                DrawDefaultInspector();

                //Check for Serializable
                var genericType = _scriptableListBase.GetGenericType;
                if (!SoapTypeUtils.IsSerializable(genericType))
                {
                    SoapEditorUtils.DrawSerializationError(genericType);
                    return;
                }
            }

            if (!EditorApplication.isPlaying)
                return;

            var container = (IDrawObjectsInInspector)target;
            var gameObjects = container.GetAllObjects();


            SoapInspectorUtils.DrawLine();

            if (gameObjects.Count > 0)
                DisplayAll(gameObjects);
        }

        private void DisplayAll(List<Object> objects)
        {
            GUILayout.Space(15);
            var title = $"List Count : {objects.Count}";
            GUILayout.BeginVertical(title, "window");
            foreach (var obj in objects)
                SoapInspectorUtils.DrawSelectableObject(obj, new[] { obj.name, "Select" });

            GUILayout.EndVertical();
        }

        #region Repaint

        private void OnEnable()
        {
            if (_repaintFlag)
                return;

            _scriptableBase = target as ScriptableBase;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            _repaintFlag = true;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                if (_scriptableBase == null)
                    _scriptableBase = target as ScriptableBase;
                _scriptableBase.RepaintRequest += OnRepaintRequested;
            }
            else if (obj == PlayModeStateChange.EnteredEditMode)
                _scriptableBase.RepaintRequest -= OnRepaintRequested;
        }

        private void OnRepaintRequested() => Repaint();
    }

    #endregion
}