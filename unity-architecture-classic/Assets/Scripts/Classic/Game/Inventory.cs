using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    public class Inventory : MonoBehaviour
    {
        [field: SerializeField]
        public List<StoreItem> storeItems { get; private set; } = new();
        [field:SerializeField]
        public List<ChestItem> chestItems { get; private set; } = new();
        
        public UnityEvent<StoreItem> onStoreItemAdded = new();
        public UnityEvent<ChestItem> onChestItemAdded = new();
        
        public void AddStoreItem(StoreItem item)
        {
            storeItems.Add(item);
            onStoreItemAdded.Invoke(item);
        }
        
        public void AddChestItem(ChestItem item)
        {
            chestItems.Add(item);
            onChestItemAdded.Invoke(item);
        }

        public void ClearAll()
        {
            storeItems.Clear();
            chestItems.Clear();
        }
    }
}