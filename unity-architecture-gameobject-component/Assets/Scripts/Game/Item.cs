using System;
using UnityEngine;

namespace GameObjectComponent.Game
{
    [Serializable]
    public class Item
    {
        public Sprite sprite;
        public string itemName;
        public int tier;
        public Modifier[] modifiers;
    }
}