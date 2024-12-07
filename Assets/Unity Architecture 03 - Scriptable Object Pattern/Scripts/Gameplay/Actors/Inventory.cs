using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class Inventory : MonoBehaviour
    {
        [field:SerializeField] public List<ChestItem> items { get; private set; } = new();
        public UnityEvent<ChestItem> onItemAdded = new();

        public void AddItem(ChestItem chestItem)
        {
            items.Add(chestItem);
            onItemAdded.Invoke(chestItem);
        }
    }
}