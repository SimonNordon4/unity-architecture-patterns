using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(ShowInline))]
    public class InlineScriptableObject : PropertyDrawer
    {
        private Editor _editor;
        private bool _expand;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return base.GetPropertyHeight(property, label);

            if (property.objectReferenceValue is not ScriptableObject)
                return base.GetPropertyHeight(property, label);

            return base.GetPropertyHeight(property, label);

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 16f), property);

            if (property.objectReferenceValue == null)
                return;

            if (property.objectReferenceValue is not ScriptableObject)
                return;

            _expand = EditorGUI.Foldout(new Rect(position.x, position.y, position.x + 10f, position.height), _expand, "");

            if (!_expand)
                return;

            EditorGUI.indentLevel++;
            property.serializedObject.Update();
            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
            _editor.OnInspectorGUI();
            property.serializedObject.ApplyModifiedProperties();
        }

        
    }