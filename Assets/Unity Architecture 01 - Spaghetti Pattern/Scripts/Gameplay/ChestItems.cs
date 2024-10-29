using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class ChestItems
    {
        public string jsonFile;
        public int tier = 1;

        public List<ChestItem> chestItems = new();

        public void Load()
        {
            if (string.IsNullOrEmpty(jsonFile)) return;
            
            var filePath = System.IO.Path.Combine(Application.persistentDataPath, jsonFile);
            if (!System.IO.File.Exists(filePath)) return;
            var jsonContent = System.IO.File.ReadAllText(filePath);
            var loadedItems = JsonUtility.FromJson<ChestItems>(jsonContent);
            if (loadedItems == null) return;
            chestItems = loadedItems.chestItems;
            tier = loadedItems.tier;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(jsonFile)) return;
            
            var filePath = System.IO.Path.Combine(Application.persistentDataPath, jsonFile);
            var jsonContent = JsonUtility.ToJson(this, true);
            System.IO.File.WriteAllText(filePath, jsonContent);
        }
    }
}