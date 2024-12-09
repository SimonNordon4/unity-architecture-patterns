#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ScriptableObjectContextMenu
{
    [MenuItem("Assets/Create Scriptable Object", true)]
    private static bool ValidateCreateScriptableObject()
    {
        var script = Selection.activeObject as MonoScript;
        if (!script) return false;

        var type = script.GetClass();
        if (type == null) return false;

        return type.IsSubclassOf(typeof(ScriptableObject));
    }

    [MenuItem("Assets/Create Scriptable Object")]
    private static void CreateScriptableObjectInstance()
    {
        var script = (MonoScript)Selection.activeObject;
        var type = script.GetClass();
        var instance = ScriptableObject.CreateInstance(type);

        var scriptPath = AssetDatabase.GetAssetPath(script);
        var directory = Path.GetDirectoryName(scriptPath);
        var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, type.Name + ".asset"));

        AssetDatabase.CreateAsset(instance, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = instance;
    }
}
#endif