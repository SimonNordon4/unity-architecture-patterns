using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class Inventory : MonoBehaviour
    {
        [field:SerializeField] public List<ChestItem> items { get; private set; } = new();
        public UnityEvent<ChestItem> onItemAdded = new();

        public void AddItem(ChestItem chestItem)
        {
            Debug.Log($"Chest item added: {chestItem.itemName}");
            items.Add(chestItem);
            onItemAdded.Invoke(chestItem);
        }
    }
}