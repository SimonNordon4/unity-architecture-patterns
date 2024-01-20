using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Definitions;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIShowInventory : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private RectTransform inventoryItemContainer;
        [SerializeField] private UIChestItemHoverImage chestItemHoverImagePrefab;
        
        private readonly List<UIChestItemHoverImage> _itemHoverImages = new();
        
        private void OnEnable()
        {
            PopulateInventoryUI();
        }
        
        private void OnDisable()
        {
            ClearItems();
        }

        private void ClearItems()
        {
            // Clear the existing UI items
            foreach (var itemHoverImage in _itemHoverImages)
            {
                Destroy(itemHoverImage.gameObject);
            }
            _itemHoverImages.Clear();
        }

        private void PopulateInventoryUI()
        {
            // Create a dictionary of items and their counts
            Dictionary<ChestItemDefinition, int> itemCounts = new Dictionary<ChestItemDefinition, int>();

            foreach (var item in inventory.items.Where(item => !itemCounts.TryAdd(item, 1)))
            {
                itemCounts[item]++;
            }

            // Create new UI items
            foreach (var pair in itemCounts)
            {
                var itemHoverImage = Instantiate(chestItemHoverImagePrefab, inventoryItemContainer);
                itemHoverImage.Construct(pair.Key, pair.Value);
                itemHoverImage.Initialize();
                _itemHoverImages.Add(itemHoverImage);
            }
        }
    }
}