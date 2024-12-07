using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIInventoryDisplay : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private UIChestItemHoverImage chestItemHoverImagePrefab;

        private readonly List<UIChestItemHoverImage> chestItemHoverImages = new();

        private void OnEnable()
        {
            ClearItemsDisplay();
            PopulateItems();
            if (inventory != null)
                inventory.onItemAdded.AddListener(OnItemAdded);
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.onItemAdded.RemoveListener(OnItemAdded);
        }

        private void ClearItemsDisplay()
        {
            foreach (UIChestItemHoverImage chestItemHoverImage in chestItemHoverImages)
            {
                Destroy(chestItemHoverImage.gameObject);
            }
            chestItemHoverImages.Clear();
        }

        private void PopulateItems()
        {
            if (inventory == null)
                return;

            var items = inventory.items;
            var itemCounts = new Dictionary<ChestItem, int>();

            foreach (var item in items)
            {
                if (!itemCounts.ContainsKey(item))
                    itemCounts[item] = 0;
                itemCounts[item]++;
            }

            foreach (var kvp in itemCounts)
            {
                var hoverImage = Instantiate(chestItemHoverImagePrefab, itemContainer);
                hoverImage.Initialize(kvp.Key, kvp.Value);
                chestItemHoverImages.Add(hoverImage);
            }
        }

        private void OnItemAdded(ChestItem newItem)
        {
            // Optionally handle item addition dynamically if desired
            ClearItemsDisplay();
            PopulateItems();
        }
    }
}
