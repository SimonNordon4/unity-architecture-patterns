using System;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameObjectComponent.Items;
using UnityEngine;

namespace GameplayComponents.Actor
{
    public class Inventory : GameplayComponent
    {
        [field:SerializeField] public List<ItemDefinition> items { get; private set; } = new();
        public event Action<ItemDefinition> OnItemAdded;

        public void AddItem(ItemDefinition item)
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