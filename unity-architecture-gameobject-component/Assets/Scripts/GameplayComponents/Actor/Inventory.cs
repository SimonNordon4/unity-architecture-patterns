using System;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameObjectComponent.Items;
using UnityEngine;

namespace GameplayComponents.Actor
{
    public class Inventory : GameplayComponent
    {
        [field:SerializeField] public List<ChestItemDefinition> items { get; private set; } = new();
        public event Action<ChestItemDefinition> OnItemAdded;

        public void AddItem(ChestItemDefinition chestItem)
        {
            Debug.Log($"Added {chestItem.name} to {gameObject.name}'s inventory");
            items.Add(chestItem);
            OnItemAdded?.Invoke(chestItem);
        }

        public override void OnGameEnd()
        {
            items.Clear();
        }
    }
}