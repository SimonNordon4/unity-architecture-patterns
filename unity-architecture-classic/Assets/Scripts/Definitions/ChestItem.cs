using System;
using System.Diagnostics;
using Classic.Game;
using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "ChestItem", menuName = "Prototype/ChestItem", order = 1)]
    public class ChestItem : ScriptableObject
    {
        private Item item;
        public Sprite sprite;
        public string itemName = "New Item";
        public int tier = 1;
        public int spawnChance = 100;
        public Modifier[] modifiers;

        public Item GetItem()
        {
            var newItem = new Item();
            newItem.sprite = sprite;
            newItem.itemName = itemName;
            newItem.tier = tier;
            newItem.modifiers = modifiers;
            return newItem;
        }

        #if UNITY_EDITOR
        [ContextMenu("Rename Image")]
        public void RenameImage()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(sprite);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            // rename image to the name of this scriptable object
            UnityEditor.AssetDatabase.RenameAsset(path, this.name);
        }
        #endif
    }
