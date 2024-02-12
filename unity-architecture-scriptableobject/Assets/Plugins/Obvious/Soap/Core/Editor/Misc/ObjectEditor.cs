using UnityEditor;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor
{
    /// <summary>
    /// Needed to draw custom editors.
    /// Note: Delete this class if using NaughtyAttributes
    /// </summary>
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    internal class ObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //when using AddComponent, the target object is null before the domains reloads.
            if (serializedObject.targetObject == null)
                return;
            
            var targetType = serializedObject.targetObject.GetType();
            var customEditorType = typeof(CustomEditor);
            var customEditors = targetType.GetCustomAttributes(customEditorType, true);
        
            if (customEditors.Length == 0)
            {
                DrawDefaultInspector();
            }
            else
            {
                // Custom editor exists, handle it accordingly
                base.OnInspectorGUI();
            }
        }
    }
}
