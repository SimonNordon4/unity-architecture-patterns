using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.App;
using GameplayComponents.Actor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GameObjectComponent.UI
{
    public class StoreMenuManager : MonoBehaviour
    {
        [SerializeField] private Store store;
        [SerializeField] private Gold playerGold;
        [SerializeField] private RectTransform storeItemContainer;
        [SerializeField] private UIStoreItem storeItemUIPrefab;
        private readonly List<UIStoreItem> _storeItemUis = new List<UIStoreItem>();

        private void OnEnable()
        {
            playerGold.OnGoldChanged += UpdateStoreMenu;
            Init();
        }

        private void UpdateStoreMenu(int newGoldAmount)
        {
            foreach (var ui in _storeItemUis)
            {
                ui.UpdateAffordability(newGoldAmount);
            }
        }

        public void UpdateStoreMenu()
        {
            Clear();
            Init();
        }

        private void Init()
        {
            foreach (var storeItem in store.purchasedStoreItems)
            {
                var storeItemUi = Instantiate(storeItemUIPrefab, storeItemContainer);
                storeItemUi.Initialize(storeItem, playerGold.amount);
                _storeItemUis.Add(storeItemUi);
                storeItemUi.purchaseButton.onClick.AddListener(() => store.PurchaseUpgrade(storeItem));
            }
        }

        private void Clear()
        {
            foreach (var storeItemUi in _storeItemUis.Where(storeItemUi => storeItemUi != null))
            {
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