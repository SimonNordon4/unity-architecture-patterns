using System;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Combat
{
    public class Inventory : GameplayComponent
    {
        [field:SerializeField] public List<Item> items { get; private set; } = new();
        public event Action<Item> OnItemAdded;

        public void AddItem(Item item)
        {
            items.Add(item);
            OnItemAdded?.Invoke(item);
        }
        
        public void ClearAll()
        {
            items.Clear();
        }
    }
}