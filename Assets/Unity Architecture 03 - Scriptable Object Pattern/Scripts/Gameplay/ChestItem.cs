using System;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [CreateAssetMenu(menuName="SpaghettiPattern/ChestItem")]
    public class ChestItem : ScriptableObject
    {
        public string itemName = "New Item";
        public Sprite sprite;
        public Modifier[] modifiers;
        public int tier = 1;
    }
}