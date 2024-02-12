using UnityEngine;
using UnityEditor;

public class CreateScriptableObjectAsset
{
    [MenuItem("Assets/Create ScriptableObject", true)]
    private static bool CreateScriptableObjectValidation()
    {
        // Validate the menu item is only available when a single ScriptableObject script is selected.
        var script = Selection.activeObject as MonoScript;
        return script != null && script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(ScriptableObject));
    }

    [MenuItem("Assets/Create ScriptableObject")]
    private static void CreateScriptableObject()
    {
        var script = Selection.activeObject as MonoScript;
        var scriptType = script.GetClass();

        if (scriptType == null || !scriptType.IsSubclassOf(typeof(ScriptableObject)))
        {
            Debug.LogError("The selected script is not a ScriptableObject.");
            return;
        }

        ScriptableObject asset = ScriptableObject.CreateInstance(scriptType);

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path = path.Replace(".cs", ".asset");
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}