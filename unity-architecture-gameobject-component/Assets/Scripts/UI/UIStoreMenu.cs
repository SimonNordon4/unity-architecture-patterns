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
            Debug.Log(store.purchasedStoreItems.Count);
            
            foreach(var storeItem in store.purchasedStoreItems)
            {
                var storeItemUi = Instantiate(storeItemUIPrefab, storeItemContainer);
                storeItemUi.Construct(store, playerGold, storeItem);
                storeItemUi.Init();
                _storeItemUis.Add(storeItemUi);
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