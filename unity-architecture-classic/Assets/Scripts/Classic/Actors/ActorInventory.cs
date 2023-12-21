using System;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;

namespace Classic.Actors
{
    public class ActorInventory : ActorComponent
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