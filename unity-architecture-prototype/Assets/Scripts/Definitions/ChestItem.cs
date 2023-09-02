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
            itemName = this.name.Remove(0,3);
            var tChar = this.name[1];

            tier = tChar switch
            {
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                _ => 1
            };
        }
    }
