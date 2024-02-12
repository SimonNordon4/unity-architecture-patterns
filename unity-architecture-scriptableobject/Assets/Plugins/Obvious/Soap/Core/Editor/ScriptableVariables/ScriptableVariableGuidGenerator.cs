using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    class ScriptableVariableGuidGenerator : AssetPostprocessor
    {
        //this gets cleared every time the domain reloads
        private static readonly HashSet<string> _guidsCache = new HashSet<string>();

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var isInitialized = SessionState.GetBool("initialized", false);
            if (!isInitialized)
            {
                RegenerateAllGuids();
                SessionState.SetBool("initialized", true);
            }
            else
            {
                OnAssetCreatedOrSaved(importedAssets);
                OnAssetDeleted(deletedAssets);
                OnAssetMoved(movedFromAssetPaths, movedAssets);
            }
        }
        
        private static void RegenerateAllGuids()
        {
            var scriptableVariableBases = SoapEditorUtils.FindAll<ScriptableVariableBase>();
            foreach (var scriptableVariable in scriptableVariableBases)
            {
                if (scriptableVariable.SaveGuid != SaveGuidType.Auto)
                    continue;
                scriptableVariable.Guid = GenerateGuid(scriptableVariable);
                _guidsCache.Add(scriptableVariable.Guid);
            }
        }

        private static void OnAssetCreatedOrSaved(string[] importedAssets)
        {
            foreach (var assetPath in importedAssets)
            {
                if (_guidsCache.Contains(assetPath))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<ScriptableVariableBase>(assetPath);
                if (asset == null || asset.SaveGuid != SaveGuidType.Auto)
                    continue;
                
                asset.Guid = GenerateGuid(asset);
                _guidsCache.Add(asset.Guid);
            }
        }

        private static void OnAssetDeleted(string[] deletedAssets)
        {
            foreach (var assetPath in deletedAssets)
            {
                if (!_guidsCache.Contains(assetPath))
                    continue;

                _guidsCache.Remove(assetPath);
                //Debug.Log(assetPath + " was removed from cache");
            }
        }

        private static void OnAssetMoved(string[] movedFromAssetPaths, string[] movedAssets)
        {
            OnAssetDeleted(movedFromAssetPaths);
            OnAssetCreatedOrSaved(movedAssets);
        }
        
        private static string GenerateGuid(ScriptableObject scriptableObject)
        {
            var path = AssetDatabase.GetAssetPath(scriptableObject);
            var guid = AssetDatabase.AssetPathToGUID(path);
            return guid;
        }
    }
}