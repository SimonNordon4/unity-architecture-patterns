using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableVariableBase), true)]
    public class ScriptableVariableDrawer : UnityEditor.Editor
    {
        private ScriptableBase _scriptableBase = null;
        private ScriptableVariableBase _scriptableVariable = null;
        private static bool _repaintFlag;

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            //Check for Serializable
            if (_scriptableVariable == null)
                _scriptableVariable = target as ScriptableVariableBase;
            var genericType = _scriptableVariable.GetGenericType;
            if (!SoapTypeUtils.IsSerializable(genericType))
                SoapEditorUtils.DrawSerializationError(genericType);

            var settings = SoapEditorUtils.GetOrCreateSoapSettings();
            if (settings == null)
                return;
            if (settings.VariableDisplayMode == EVariableDisplayMode.Minimal)
                DrawMinimal();
            else
                DrawDefault();

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);

            if (!EditorApplication.isPlaying)
                return;

            var container = (IDrawObjectsInInspector)target;
            var objects = container.GetAllObjects();

            SoapInspectorUtils.DrawLine();

            if (objects.Count > 0)
                DisplayAll(objects);
        }

        private void DrawMinimal()
        {
            serializedObject.DrawOnlyField("_value", false);
        }

        private void DrawDefault()
        {
            serializedObject.DrawOnlyField("m_Script", true);
            var propertiesToHide = CanShowMinMaxProperty(target)
                ? new[] { "m_Script" ,"_guid","_saveGuid" }
                : new[] { "m_Script", "_minMax" ,"_guid" ,"_saveGuid" };
            serializedObject.DrawCustomInspector(propertiesToHide);

            if (GUILayout.Button("Reset to initial value"))
            {
                var so = (IReset)target;
                so.ResetToInitialValue();
            }
        }

        private bool CanShowMinMaxProperty(Object targetObject)
        {
            return IsIntClamped(targetObject) || IsFloatClamped(targetObject);
        }

        private bool IsIntClamped(Object targetObject)
        {
            var intVariable = targetObject as IntVariable;
            return intVariable != null && intVariable.IsClamped;
        }

        private bool IsFloatClamped(Object targetObject)
        {
            var floatVariable = targetObject as FloatVariable;
            return floatVariable != null && floatVariable.IsClamped;
        }

        private void DisplayAll(List<Object> objects)
        {
            GUILayout.Space(15);
            var title = $"Objects reacting to OnValueChanged Event : {objects.Count}";
            GUILayout.BeginVertical(title, "window");
            foreach (var obj in objects)
            {
                var text = $"{obj.name}  ({obj.GetType().Name})";
                SoapInspectorUtils.DrawSelectableObject(obj, new[] { text, "Select" });
            }

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
            //TODO: investigate this
            if (target == null)
                return;

            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                if (_scriptableBase == null)
                    _scriptableBase = target as ScriptableBase;
                _scriptableBase.RepaintRequest += OnRepaintRequested;
            }
            else if (obj == PlayModeStateChange.ExitingPlayMode)
                _scriptableBase.RepaintRequest -= OnRepaintRequested;
        }

        private void OnRepaintRequested() => Repaint();

        #endregion
    }
}