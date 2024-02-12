using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableListBase), true)]
    public class ScriptableListPropertyDrawer : ScriptableBasePropertyDrawer
    {
        private SerializedObject _serializedObject;
        private ScriptableListBase _scriptableListBase;

        protected override void DrawUnExpanded(Rect position, SerializedProperty property, GUIContent label,
            Object targetObject)
        {
            if (_serializedObject == null || _serializedObject.targetObject != targetObject)
                _serializedObject = new SerializedObject(targetObject);

            _serializedObject.UpdateIfRequiredOrScript();
            base.DrawUnExpanded(position, property, label, targetObject);
            _serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawShortcut(Rect position, SerializedProperty property, Object targetObject)
        {
            if (_scriptableListBase == null)
                _scriptableListBase = _serializedObject.targetObject as ScriptableListBase;

            var genericType = _scriptableListBase.GetGenericType;
            if (!SoapTypeUtils.IsSerializable(genericType))
            {
                SoapEditorUtils.DrawSerializationError(genericType, position);
                return;
            }

            var value = _serializedObject.FindProperty("_list");
            var count = value.arraySize;
            EditorGUI.LabelField(position, "Count: " + count);
        }
    }
}