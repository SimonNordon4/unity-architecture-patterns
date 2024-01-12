using System;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class Store : PersistentComponent
    {
        [field:SerializeField] public List<StoreItemDefinition> storeItemDefinitions { get; private set; }
        [SerializeField] private Gold playerGold;

        [field: SerializeField]
        public List<StoreItem> purchasedStoreItems { get; private set; } = new();
        private StoreItemsSave _storeItemsSave;
    

        [ContextMenu("Populate Store Items")]
        public void PopulateStoreItems()
        {
            purchasedStoreItems = new List<StoreItem>();
            
            foreach (var storeItemDefinition in storeItemDefinitions)
            {
                purchasedStoreItems.Add(new StoreItem
                {
                    storeItemDefinition = storeItemDefinition,
                    upgradesPurchased = 0
                });
            }
            
            
        }

        public override void Save()
        {
            // save purchasedStoreItems
            _storeItemsSave = new StoreItemsSave(purchasedStoreItems);
            var json = JsonUtility.ToJson(_storeItemsSave);
            PlayerPrefs.SetString($"purchasedStoreItems_{id}", json);
        }

        public override void Load()
        {
            if (!PlayerPrefs.HasKey($"purchasedStoreItems_{id}"))
            {
                PopulateStoreItems();
                return;
            }
            
            var json = PlayerPrefs.GetString($"purchasedStoreItems_{id}");
            _storeItemsSave = JsonUtility.FromJson<StoreItemsSave>(json);

            if (_storeItemsSave == null)
            {
                PopulateStoreItems();
                return;
            }
            
            purchasedStoreItems = new List<StoreItem>(_storeItemsSave.storeItems);
        }

        public void PurchaseUpgrade(StoreItem storeItem)
        {
            if (playerGold.amount >= storeItem.storeItemDefinition.upgrades[storeItem.upgradesPurchased].cost)
            {
                playerGold.ChangeGold(-storeItem.storeItemDefinition.upgrades[storeItem.upgradesPurchased].cost);
                storeItem.upgradesPurchased++;
            }
            Save();
        }

        private void OnEnable()
        {
            Load();
        }
        
        private void OnDisable()
        {
            Save();
        }

        [Serializable]
        public class StoreItemsSave
        {
            public StoreItem[] storeItems;
            
            public StoreItemsSave(List<StoreItem> storeItems)
            {
                this.storeItems = storeItems.ToArray();
            }
        }
    }
}