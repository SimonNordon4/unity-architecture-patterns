using System;
using System.Diagnostics;
using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "ChestItem", menuName = "Prototype/ChestItem", order = 1)]
    public class ChestItem : ScriptableObject
    {
        public Sprite sprite;
        public string itemName = "New Item";
        public int tier = 1;
        public int spawnChance = 100;
        public Modifier[] modifiers;

        private void OnValidate()
        {
            
        }
    }
