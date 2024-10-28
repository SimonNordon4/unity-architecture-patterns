using System;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class ChestItem
    {
        public Sprite sprite;
        public string itemName = "New Item";
        public int tier = 1;
        public int spawnChance = 100;
        public Modifier[] modifiers;
    }
}