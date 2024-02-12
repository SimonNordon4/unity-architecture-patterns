namespace Obvious.Soap.Editor
{
    using UnityEditor;

    [InitializeOnLoad]
    static class ScriptableBaseEditorHeader
    {
        private static SoapSettings _soapSettings;

        static ScriptableBaseEditorHeader()
        {
            _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            Editor.finishedDefaultHeaderGUI += DisplayCategory;
        }

        private static void DisplayCategory(Editor editor)
        {
            if (!EditorUtility.IsPersistent(editor.target))
                return;

            if (editor.targets.Length > 1)
            {
                //If there is more than one target, we check if they are all ScriptableBase
                foreach (var target in editor.targets)
                {
                    var scriptableBase = target as ScriptableBase;
                    if (scriptableBase == null)
                        return;
                }

                //Only draws the category for the selected target
                var scriptableTarget = editor.target as ScriptableBase;
                if (DrawCategory(scriptableTarget))
                {
                    //Assign the category to all the targets
                    foreach (var target in editor.targets)
                    {
                        if (target == editor.target)
                            continue;
                        var scriptableBase = target as ScriptableBase;
                        Undo.RecordObject(scriptableBase, "Change Category");
                        scriptableBase.CategoryIndex = scriptableTarget.CategoryIndex;
                        EditorUtility.SetDirty(scriptableBase);
                    }
                }
            }
            else
            {
                var scriptableBase = editor.target as ScriptableBase;
                if (scriptableBase == null)
                    return;

                DrawCategory(scriptableBase);
            }

            bool DrawCategory(ScriptableBase scriptableBase)
            {
                if (_soapSettings == null)
                    _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
                var categories = _soapSettings.Categories.ToArray();
                var totalRect = EditorGUILayout.GetControlRect();
                var controlRect = EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent("Category:"));
                EditorGUI.BeginChangeCheck();
                int newCategoryIndex = EditorGUI.Popup(controlRect, scriptableBase.CategoryIndex, categories);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(scriptableBase, "Change Category");
                    scriptableBase.CategoryIndex = newCategoryIndex; // Apply the change
                    EditorUtility.SetDirty(scriptableBase);
                    return true;
                }

                return false;
            }
        }
    }
}