using System.Collections.Generic;
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class Store : PersistentComponent
    {
        [SerializeField] private List<StoreItemDefinition> storeItemDefinitions;
        [SerializeField] private Gold playerGold;

        public List<StoreItem> purchasedStoreItems { get; private set; }


        private void PopulateStoreItems()
        {
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
            PlayerPrefs.SetString($"purchasedStoreItems_{id}", JsonUtility.ToJson(purchasedStoreItems));
        }

        public override void Load()
        {
            if (PlayerPrefs.HasKey($"purchasedStoreItems_{id}"))
            {
                purchasedStoreItems = JsonUtility.FromJson<List<StoreItem>>(PlayerPrefs.GetString($"purchasedStoreItems_{GetInstanceID()}"));
            }
            else
            {
                PopulateStoreItems();
            }
        }

        public void PurchaseUpgrade(StoreItem storeItem)
        {
            if (playerGold.amount >= storeItem.storeItemDefinition.upgrades[storeItem.upgradesPurchased].cost)
            {
                playerGold.AddGold(-storeItem.storeItemDefinition.upgrades[storeItem.upgradesPurchased].cost);
                storeItem.upgradesPurchased++;
            }

            Save();
        }
    }
}