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
        public List<StoreItem> storeItems = new();

        public bool debugSkipLoad = false;
        
        public void AddGold(int amount)
        {
            totalGold += amount;
        }

        public void PurchaseStoreItem(StoreItem item)
        {
            totalGold -= (int)(item.price + item.currentTier * item.priceIncreasePerTier);
            // find the item in the store items list
            var storeItem = storeItems.Find(x => x.itemName == item.itemName);
            // increase the tier
            storeItem.currentTier = Mathf.Clamp(storeItem.currentTier + 1,0, storeItem.tiers);
            
            foreach(var mod in storeItem.modifiers)
            {
                mod.modifierValue = mod.modifierValue + storeItem.tierModifierMultiplier * storeItem.currentTier;
            }
            // refresh the ui
            FindObjectOfType<StoreMenuManager>().UpdateStoreMenu();
        }

        private void OnEnable()
        {
            if(debugSkipLoad) return;
            
            Load();

        }
        private void OnDisable()
        {
            Save();
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
