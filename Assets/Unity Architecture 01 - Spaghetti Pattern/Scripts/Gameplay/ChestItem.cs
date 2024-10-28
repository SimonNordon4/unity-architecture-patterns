using System;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class ChestItem
    {
        public string itemName = "New Item";
        public Sprite sprite;
        public Modifier[] modifiers;
        public int tier = 1;
    }
}