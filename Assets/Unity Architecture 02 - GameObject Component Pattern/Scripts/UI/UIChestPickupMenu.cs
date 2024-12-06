using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIChestPickupMenu : MonoBehaviour
    {
        [SerializeField]private ChestPickupHandler chestPickupHandler;
        public RectTransform chestItemButtonContainer;
        public UIChestItemButton chestItemButtonPrefab;
        private readonly List<UIChestItemButton> _chestItemButtons = new();
        public UIWasdButtonSelector wasdButtonSelector;

        public void OnEnable()
        {
            var chest = chestPickupHandler.CurrentChest;

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
            chestPickupHandler.SelectItem(item);
            gameObject.SetActive(false);
        }
    }
}
