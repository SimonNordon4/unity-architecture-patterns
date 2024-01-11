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
            Debug.Log(store.purchasedStoreItems.Count);
            
            for(var i = 0; i < store.purchasedStoreItems.Count; i++)
            {
                var storeItemUi = Instantiate(storeItemUIPrefab, storeItemContainer);
                storeItemUi.Initialize(store.purchasedStoreItems[i], playerGold.amount);
                _storeItemUis.Add(storeItemUi);
                var x = i;
                storeItemUi.purchaseButton.onClick.AddListener(() => store.PurchaseUpgrade(store.purchasedStoreItems[x]));
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