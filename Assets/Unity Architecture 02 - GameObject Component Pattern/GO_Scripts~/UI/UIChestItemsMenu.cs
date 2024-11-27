using System.Collections.Generic;
using GameObjectComponent.Items;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIChestItemsMenu : MonoBehaviour
    {
        [SerializeField] private ChestOpenHandler chestOpenHandler;
        [SerializeField] private RectTransform chestItemButtonContainer;
        [SerializeField] private UIChestItemButton chestItemButtonPrefab;
        
        private readonly List<UIChestItemButton> _chestItemButtons = new();

        private void OnEnable()
        {
            var chest = chestOpenHandler.currentChest;

            foreach (var chestItem in chest.chestItems)
            {
                var button = Instantiate(chestItemButtonPrefab, chestItemButtonContainer);
                button.Initialize(chestItem);
                button.GetComponent<Button>().onClick.AddListener(() => chestOpenHandler.SelectItem(chestItem));
                _chestItemButtons.Add(button);
            }
        }

        private void OnDisable()
        {
            foreach (var button in _chestItemButtons)
            {
                Destroy(button.gameObject);
            }
            
            _chestItemButtons.Clear();
        }
    }
}