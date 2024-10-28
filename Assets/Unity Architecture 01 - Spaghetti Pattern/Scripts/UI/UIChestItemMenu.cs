using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIChestItemMenu : MonoBehaviour
    {
        [Header("References")]
        public ChestManager chestManager;
        public GameManager gameManager;

        [Header("Chest UI")]
        public RectTransform chestItemButtonContainer;
        public UIChestItemButton chestItemButtonPrefab;
        private readonly List<UIChestItemButton> _chestItemButtons = new();
        public UIWasdButtonSelector wasdButtonSelector;

        public void OnEnable()
        {
            var chest = chestManager.currentChest;

            gameManager.isPaused = true;

            foreach (var cib in _chestItemButtons) Destroy(cib.gameObject);
            _chestItemButtons.Clear();

            foreach (var item in chest.items)
            {
                var newChestItemButton = Instantiate(chestItemButtonPrefab, chestItemButtonContainer);
                newChestItemButton.Initialize(item, this);
                _chestItemButtons.Add(newChestItemButton);
            }

            wasdButtonSelector.buttons.Clear();
            foreach (var chestItemUI in _chestItemButtons)
                wasdButtonSelector.buttons.Add(chestItemUI.GetComponent<Button>());
        }

        public void OnItemSelected(ChestItem item)
        {
            chestManager.ApplyItem(item);
            gameManager.isPaused = false;
        }
    }
}
