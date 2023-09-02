using System;
using System.Collections.Generic;
using TMPro;
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
        
        [Header("Gold")]
        public int totalGold;

        [Header("Statistics")]
        public StatisticsSave statistics = new();
        
        [Header("Store")]
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
            
            
            json = JsonUtility.ToJson(statistics);
            PlayerPrefs.SetString("statistics", json);
            
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
            
            json = PlayerPrefs.GetString("statistics");
            if (string.IsNullOrEmpty(json))
            {
                statistics = new StatisticsSave();
                return;
            }
            statistics = JsonUtility.FromJson<StatisticsSave>(json);
        }
        
        public void CheckIfHighestStat(StatType type, float value)
        {
            switch (type)
            {
                case StatType.PistolDamage:
                    if (value > statistics.highestPistolDamage)
                        statistics.highestPistolDamage = (int) value;
                    break;
                case StatType.PistolRange:
                    if (value > statistics.highestPistolRange)
                        statistics.highestPistolRange = (int) value;
                    break;
                case StatType.PistolFireRate:
                    if (value > statistics.highestPistolFireRate)
                        statistics.highestPistolFireRate = (int) value;
                    break;
                case StatType.PistolKnockBack:
                    if (value > statistics.highestPistolKnockBack)
                        statistics.highestPistolKnockBack = (int) value;
                    break;
                case StatType.PistolPierce:
                    if (value > statistics.highestPistolPierce)
                        statistics.highestPistolPierce = (int) value;
                    break;
                case StatType.PlayerHealth:
                    if (value > statistics.highestPlayerHealth)
                        statistics.highestPlayerHealth = (int) value;
                    break;
                case StatType.PlayerSpeed:
                    if (value > statistics.highestPlayerSpeed)
                        statistics.highestPlayerSpeed = (int) value;
                    break;
                case StatType.SwordDamage:
                    if (value > statistics.highestSwordDamage)
                        statistics.highestSwordDamage = (int) value;
                    break;
                case StatType.SwordRange:
                    if (value > statistics.highestSwordRange)
                        statistics.highestSwordRange = (int) value;
                    break;
                case StatType.SwordAttackSpeed:
                    if (value > statistics.highestSwordAttackSpeed)
                        statistics.highestSwordAttackSpeed = (int) value;
                    break;
                case StatType.SwordKnockBack:
                    if (value > statistics.highestSwordKnockBack)
                        statistics.highestSwordKnockBack = (int) value;
                    break;
                case StatType.SwordArc:
                    if (value > statistics.highestSwordArc)
                        statistics.highestSwordArc = (int) value;
                    break;
                case StatType.HealthPackSpawnRate:
                    if (value > statistics.highestHealthPackSpawnRate)
                        statistics.highestHealthPackSpawnRate = (int) value;
                    break;
                case StatType.Luck:
                    if (value > statistics.highestLuck)
                        statistics.highestLuck = (int) value;
                    break;
            }
        }
    }


    [Serializable]
    public struct AccountSave
    {
        public int totalGold;
        public StoreItem[] storeItems;
    }

[Serializable]
public struct StatisticsSave
{
    public int totalKills;
    public int totalBossKills;
    public int totalGoldEarned;
    public int totalChestsOpened;
    public int totalDeaths;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalDamageHealed;

    public int gamesWon;
    public int gamesPlayed;
    public float fastestWin;
    
    // statistics?
    public int highestPistolDamage;
    public int highestPistolRange;
    public int highestPistolFireRate;
    public int highestPistolKnockBack;
    public int highestPistolPierce;

    public int highestPlayerSpeed;
    public int highestPlayerHealth;

    public int highestSwordDamage;
    public int highestSwordRange;
    public int highestSwordAttackSpeed;
    public int highestSwordKnockBack;
    public int highestSwordArc;

    public int highestHealthPackSpawnRate;
    public int highestLuck;

}
