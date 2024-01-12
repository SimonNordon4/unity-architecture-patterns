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


        private void PopulateStoreItems()
        {
            Debug.Log("No Store Item found, creating new.");
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
            Debug.Log("Saving store items");
            var json = JsonUtility.ToJson(purchasedStoreItems);
            Debug.Log(purchasedStoreItems.Count);
            Debug.Log(json);
            PlayerPrefs.SetString($"purchasedStoreItems_{id}", json);
        }

        public override void Load()
        {
            Debug.Log("Loading store items");
            if (PlayerPrefs.HasKey($"purchasedStoreItems_{id}"))
            {
                Debug.Log("Found store item Key");
                purchasedStoreItems = JsonUtility.FromJson<List<StoreItem>>(PlayerPrefs.GetString($"purchasedStoreItems_{GetInstanceID()}"));
                if (purchasedStoreItems == null)
                    PopulateStoreItems();
            }
            else
            {
                PopulateStoreItems();
            }
        }

        public void PurchaseUpgrade(StoreItem storeItem)
        {
            Debug.Log($"Purchasing upgrade for {storeItem.storeItemDefinition.name}");
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
    }
}