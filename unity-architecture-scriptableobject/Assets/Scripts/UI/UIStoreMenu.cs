using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.App;
using GameplayComponents.Actor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GameObjectComponent.UI
{
    public class UIStoreMenu : MonoBehaviour
    {
        [SerializeField] private Store store;
        [SerializeField] private Gold playerGold;
        [SerializeField] private RectTransform storeItemContainer;
        [SerializeField] private UIStoreItem storeItemUIPrefab;
        private readonly List<UIStoreItem> _storeItemUis = new List<UIStoreItem>();

        private void OnEnable()
        {
            Init();
        }

        public void UpdateStoreMenu()
        {
            Clear();
            Init();
        }

        private void Init()
        {
            // copy the store items into a new list
            // so we can sort them without affecting the original list.
            var sortedStoreItems = store.purchasedStoreItems
                .OrderBy(item => item.upgrades[item.currentUpgrade].cost)
                .ToArray();
            
            foreach(var storeItem in sortedStoreItems)
            {
                var storeItemUi = Instantiate(storeItemUIPrefab, storeItemContainer);
                storeItemUi.Construct(store, playerGold, storeItem);
                storeItemUi.Init();
                storeItemUi.purchaseButton.onClick.AddListener(UpdateStoreMenu);
                _storeItemUis.Add(storeItemUi);
            }
        }

        private void Clear()
        {
            foreach (var storeItemUi in _storeItemUis.Where(storeItemUi => storeItemUi != null))
            {
                storeItemUi.purchaseButton.onClick.RemoveAllListeners();
                Destroy(storeItemUi.gameObject);
            }

            _storeItemUis.Clear();
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}