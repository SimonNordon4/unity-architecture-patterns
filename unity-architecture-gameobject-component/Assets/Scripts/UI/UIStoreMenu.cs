using System.Collections.Generic;
using GameObjectComponent.App;
using GameplayComponents.Actor;
using TMPro;
using UnityEngine;


namespace GameObjectComponent.UI
{
    public class StoreMenuManager : MonoBehaviour
    {
        [SerializeField] private Store store;
        [SerializeField] private RectTransform storeItemContainer;
        [SerializeField] private StoreItemUI StoreItemUIPrefab;
        [SerializeField] private List<StoreItemUI> StoreItemUis = new List<StoreItemUI>();

        private void OnEnable()
        {
            Init();
        }

        public void UpdateStoreMenu()
        {
            Clear();
            Init();
        }

        void Init()
        {
            // Populate all the store item uis.
            var buttons = new List<UnityEngine.UI.Button>();
            // TODO: This needs to be from a list of store items.
            // foreach (var storeItem in characterInventory.storeItems)
            // {
            //     var storeItemUi = Instantiate(StoreItemUIPrefab, StoreItemContainer);
            //     storeItemUi.Initialize(storeItem);
            //     StoreItemUis.Add(storeItemUi);
            //     buttons.Add(storeItemUi.purchaseButton);
            // }
        }

        void Clear()
        {
            // Destroy all the store item uis.
            foreach (var storeItemUi in StoreItemUis)
            {
                if (storeItemUi != null)
                    Destroy(storeItemUi.gameObject);
            }

            StoreItemUis.Clear();
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}