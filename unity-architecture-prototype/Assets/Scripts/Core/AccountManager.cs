using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
    public class AccountManager : MonoBehaviour
    {
        private static AccountManager _instance;
        public static AccountManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<AccountManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        
        public int totalGold;
        
        public StoreItemConfig storeItemConfig;
        public List<StoreItem> storeItems = new();

        public bool debugSkipLoad = false;
        
        public void AddGold(int amount)
        {
            totalGold += amount;
        }

        public void PurchaseStoreItem(StoreItem item)
        {
            Debug.Log("Purchasing item: " + item.name);
            var itemPrice = item.pricePerTier[item.currentTier];
            totalGold -= itemPrice;
            // find the item in the store items list
            var storeItem = storeItems.Find(x => x.name == item.name);
            // increase the tier
            storeItem.currentTier = Mathf.Clamp(storeItem.currentTier + 1,0, storeItem.pricePerTier.Length);
            
            // refresh the ui
            FindObjectOfType<StoreMenuManager>().UpdateStoreMenu();
        }

        private void OnEnable()
        {
            storeItems = storeItemConfig.storeItems;
            if(debugSkipLoad) return;
            Load();

        }
        private void OnDisable()
        {
            Save();
            // reset all current tier values in teh scriptable object
            foreach (var storeItem in storeItemConfig.storeItems)
            {
                storeItem.currentTier = 0;
            }
        }


        private void Save()
        {
            var accountSave = new AccountSave();
            accountSave.totalGold = totalGold;
            accountSave.storeItems = storeItems.ToArray();
            
            var json = JsonUtility.ToJson(accountSave);
            PlayerPrefs.SetString("account", json);
        }

        private void Load()
        {
            var json = PlayerPrefs.GetString("account");
            if (string.IsNullOrEmpty(json))
            {
                totalGold = 0;
                return;
            }

            var accountSave = JsonUtility.FromJson<AccountSave>(json);
            totalGold = accountSave.totalGold;
            storeItems = new List<StoreItem>(accountSave.storeItems);
        }
    }

    [Serializable]
    public struct AccountSave
    {
        public int totalGold;
        public StoreItem[] storeItems;
    }


