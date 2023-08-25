using System;
using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "ChestItem", menuName = "Prototype/ChestItem", order = 1)]
    public class ChestItem : ScriptableObject
    {
        public int tier = 1;
        public int spawnChance = 100;
        public Modifier[] modifiers;
    }
