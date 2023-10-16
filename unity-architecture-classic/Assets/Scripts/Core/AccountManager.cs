using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Classic.App;
using Classic.Game;
using Definitions;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-10)]
    public class AccountManager : MonoBehaviour
    {
        public Inventory inventory;
        public Gold gold;
        
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
        
        [Header("Statistics")]
        public StatisticsSave statistics = new();
        
        [Header("Store")]
        public StoreItemConfig storeItemConfig;
        
        [Header("Achievements")]
        public AchievementSave achievementSave = new();
        public GameObject achievementPopup;
        public TextMeshProUGUI achievementPopupText;
        
        [Header("Settings")]
        public SettingsSave settingsSave = new();
        
        public bool debugSkipLoad = false;
        

        public void PurchaseStoreItem(StoreItem item)
        {
            Debug.Log("Purchasing item: " + item.name);
            var itemPrice = item.pricePerTier[item.currentTier];
            gold.RemoveGold(itemPrice);
            
            for (var i = 0; i < inventory.storeItems.Count; i++)
            {
                var buffer = inventory.storeItems[i];
                if (buffer.name == item.name)
                {
                    inventory.storeItems[i].currentTier = Mathf.Clamp(inventory.storeItems[i].currentTier + 1,0, inventory.storeItems[i].pricePerTier.Length);
                }
            }
            
            // refresh the ui
            FindObjectOfType<StoreMenuManager>().UpdateStoreMenu();
            
            //var buyAllStoreItemsAchieve = achievementSave.achievements.First(x=>x.id == AchievementName.BuyAllStoreItems);
            
            // if(buyAllStoreItemsAchieve.isCompleted)
            // {
            //     // total store items by summing up the tiers
            //     var totalStoreItems = inventory.storeItems.Sum(x => x.currentTier);
            //     // currently bought store items
            //     var boughtStoreItems = inventory.storeItems.Count(x => x.currentTier > 0);
            //     buyAllStoreItemsAchieve.progress = boughtStoreItems;
            //     if (totalStoreItems == boughtStoreItems)
            //     {
            //         buyAllStoreItemsAchieve.isCompleted = true;
            //         AchievementUnlocked(buyAllStoreItemsAchieve);
            //     }
            // }

        }

        private void OnEnable()
        {
            CreateAchievements();

            achievementPopup.SetActive(false);
            if(debugSkipLoad)
            {
                PopulateStoreItems();
                return;
            }
            Load();

        }
        private void OnDisable()
        {
            Save();
        }

        private void PopulateStoreItems()
        {
            Debug.Log("Populating store items");
            var storeItems = new StoreItem[storeItemConfig.storeItemDefinitions.Count];
            for(var i = 0; i < storeItemConfig.storeItemDefinitions.Count; i++)
            {
                storeItems[i] = new StoreItem(storeItemConfig.storeItemDefinitions[i]);
                inventory.AddStoreItem(storeItems[i]);
            }
        }

        public void Save()
        {
            var accountSave = new AccountSave();
            accountSave.storeItems = inventory.storeItems.ToArray();
            
            var json = JsonUtility.ToJson(accountSave);
            PlayerPrefs.SetString("account", json);
            
            json = JsonUtility.ToJson(statistics);
            PlayerPrefs.SetString("statistics", json);
            
            json = JsonUtility.ToJson(achievementSave);
            PlayerPrefs.SetString("achievements", json);
            
            json = JsonUtility.ToJson(settingsSave);
            PlayerPrefs.SetString("settings", json);
            
            PlayerPrefs.Save();
        }

        private void Load()
        {
            var json = PlayerPrefs.GetString("account");
            if (string.IsNullOrEmpty(json))
            {
                PopulateStoreItems();
            }
            else
            {
                var accountSave = JsonUtility.FromJson<AccountSave>(json);
                var storeItems = accountSave.storeItems;
                foreach (var t in storeItems)
                {
                    inventory.AddStoreItem(t);
                }
            }

           
            
            json = PlayerPrefs.GetString("statistics");
            if (string.IsNullOrEmpty(json))
            {
                statistics = new StatisticsSave
                {
                    fastestWin = 999f
                };
            }
            else
            {
                statistics = JsonUtility.FromJson<StatisticsSave>(json);
            }
            
            
            json = PlayerPrefs.GetString("achievements");
            if (string.IsNullOrEmpty(json))
            {
                CreateAchievements();
            }
            else
            {
                achievementSave = JsonUtility.FromJson<AchievementSave>(json);
            }
            
            json = PlayerPrefs.GetString("settings");
            if (string.IsNullOrEmpty(json))
            {
                settingsSave = new SettingsSave();
            }
            else
            {
                settingsSave = JsonUtility.FromJson<SettingsSave>(json);
            }

        }

        public void ResetProgress()
        {
            // delete all player prefs.
            PlayerPrefs.DeleteAll();
                        
            gold.SetGold(40);
            statistics = new StatisticsSave();
            CreateAchievements();
            PopulateStoreItems();
        }

        private void CreateAchievements()
        {
            Debug.Log("Creating achievements");
            // var achievements = new List<Achievement>
            // {
            //     new Achievement()
            //     {
            //         id = AchievementName.Die,
            //         uiName = "Die",
            //         goal = 1,
            //         rewardGold = 50
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill100Enemies,
            //         uiName = "Kill 100 Enemies",
            //         goal = 100,
            //         rewardGold = 50
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill1000Enemies,
            //         uiName = "Kill 1000 Enemies",
            //         goal = 1000,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill10000Enemies,
            //         uiName = "Kill 10000 Enemies",
            //         goal = 10000,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill100000Enemies,
            //         uiName = "Kill 100000 Enemies",
            //         goal = 100000,
            //         rewardGold = 10000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill100Bosses,
            //         uiName = "Kill 100 Bosses",
            //         goal = 100,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Kill1000Bosses,
            //         uiName = "Kill 1000 Bosses",
            //         goal = 1000,
            //         rewardGold = 5000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Die50Times,
            //         uiName = "Die 50 Times",
            //         goal = 50,
            //         rewardGold = 250
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Die100Times,
            //         uiName = "Die 100 Times",
            //         goal = 100,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Open100Chests,
            //         uiName = "Open 100 Chests",
            //         goal = 100,
            //         rewardGold = 250
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Open1000Chests,
            //         uiName = "Open 1000 Chests",
            //         goal = 1000,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Earn100Gold,
            //         uiName = "Earn 100 Gold",
            //         goal = 100,
            //         rewardGold = 50
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Earn1000Gold,
            //         uiName = "Earn 1000 Gold",
            //         goal = 1000,
            //         rewardGold = 250
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Earn10000Gold,
            //         uiName = "Earn 10000 Gold",
            //         goal = 10000,
            //         rewardGold = 2000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Earn100000Gold,
            //         uiName = "Earn 100000 Gold",
            //         goal = 100000,
            //         rewardGold = 10000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatTheGame,
            //         uiName = "Beat The Game",
            //         goal = 1,
            //         rewardGold = 10000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatTheGame10Times,
            //         uiName = "Beat The Game 10 Times",
            //         goal = 10,
            //         rewardGold = 25000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.WinInUnder1Hour,
            //         uiName = "Win In Under 1 Hour",
            //         goal = 3600,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.WinInUnder45Minutes,
            //         uiName = "Win In Under 45 Minutes",
            //         goal = 2700,
            //         rewardGold = 5000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.WinInUnder30Minutes,
            //         uiName = "Win In Under 30 Minutes",
            //         goal = 1800,
            //         rewardGold = 10000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BuyAllStoreItems,
            //         uiName = "Buy All Store Items",
            //         goal = 1,
            //         rewardGold = 1
            //     },
            //     new Achievement
            //     {
            //         id = AchievementName.Reach50PistolDamage,
            //         uiName = "Reach 50 Pistol Damage",
            //         goal = 50,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach25PistolRange,
            //         uiName = "Reach 25 Pistol Range",
            //         goal = 25,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach15PistolFireRate,
            //         uiName = "Reach 15 Pistol Fire Rate",
            //         goal = 15,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach10PistolKnockBack,
            //         uiName = "Reach 10 Pistol Knock Back",
            //         goal = 10,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach3PistolPierce,
            //         uiName = "Reach 3 Pistol Pierce",
            //         goal = 3,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach100PlayerHealth,
            //         uiName = "Reach 100 Player Health",
            //         goal = 100,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach10PlayerSpeed,
            //         uiName = "Reach 10 Player Speed",
            //         goal = 10,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach50SwordDamage,
            //         uiName = "Reach 50 Sword Damage",
            //         goal = 50,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach10SwordRange,
            //         uiName = "Reach 10 Sword Range",
            //         goal = 10,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach8SwordAttackSpeed,
            //         uiName = "Reach 8Sword Attack Speed",
            //         goal = 8,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach10SwordKnockBack,
            //         uiName = "Reach 10 Sword Knock Back",
            //         goal = 10,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach540SwordArc,
            //         uiName = "Reach 540 Sword Arc",
            //         goal = 540,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach10HealthPackSpawnRate,
            //         uiName = "Reach 10 Health Pack Spawn Rate",
            //         goal = 10,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach7Luck,
            //         uiName = "Reach 7 Luck",
            //         goal = 10,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach5Block,
            //         uiName = "Reach 5 Block",
            //         goal = 5,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.Reach60Dodge,
            //         uiName = "Reach 60 Dodge",
            //         goal = 60,
            //         rewardGold = 500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatRound1,
            //         uiName = "Beat Round 1",
            //         goal = 1,
            //         rewardGold = 1500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatRound2,
            //         uiName = "Beat Round 2",
            //         goal = 1,
            //         rewardGold = 2500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatRound3,
            //         uiName = "Beat Round 3",
            //         goal = 1,
            //         rewardGold = 5000
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatRound4,
            //         uiName = "Beat Round 4",
            //         goal = 1,
            //         rewardGold = 7500
            //     },
            //     new Achievement()
            //     {
            //         id = AchievementName.BeatRound5,
            //         uiName = "Beat Round 5",
            //         goal = 1,
            //         rewardGold = 10000
            //     },
            // };

            // achievementSave = new AchievementSave
            // {
            //     achievements = achievements.ToArray()
            // };
        }

        private void ProcessStatAchievement(Achievement achievement, float value, int target)
        {
            if (value > achievement.progress)
                achievement.progress = Mathf.Clamp((int)value, 0, achievement.goal);
            if(value >= target)
                AchievementUnlocked(achievement);
        }
        
        // We can also add our own custom achievements here.
        public void CheckIfHighestStat(StatType type, float value)
        {
            switch (type)
            {
                case StatType.RangedDamage:
                    if (value > statistics.highestPistolDamage)
                        statistics.highestPistolDamage = (int) value;
                    // var pistolDamage = achievementSave.achievements.First(x=>x.id == AchievementName.Reach50PistolDamage);
                    // ProcessStatAchievement(pistolDamage, value, 50);
                    break;
                case StatType.RangedRange:
                    if (value > statistics.highestPistolRange)
                        statistics.highestPistolRange = (int) value;
                    // var pistolRange = achievementSave.achievements.First(x=>x.id == AchievementName.Reach25PistolRange);
                    // ProcessStatAchievement(pistolRange, value, 25);
                    break;
                case StatType.RangedAttackSpeed:
                    if (value > statistics.highestPistolFireRate)
                        statistics.highestPistolFireRate = (int) value;
                    //(achievementSave.achievements.First(x=>x.id == AchievementName.Reach15PistolFireRate), value, 15);
                    break;
                case StatType.RangedKnockBack:
                    if (value > statistics.highestPistolKnockBack)
                        statistics.highestPistolKnockBack = (int) value;
                    // var pistolKnockBack = achievementSave.achievements.First(x=>x.id == AchievementName.Reach10PistolKnockBack);
                    // ProcessStatAchievement(pistolKnockBack, value, 10);
   
                    break;
                case StatType.RangedPierce:
                    if (value > statistics.highestPistolPierce)
                        statistics.highestPistolPierce = (int) value;
                    // var pistolPierce = achievementSave.achievements.First(x=>x.id == AchievementName.Reach3PistolPierce);
                    // ProcessStatAchievement(pistolPierce, value, 3);
                    break;
                case StatType.MaxHealth:
                    if (value > statistics.highestPlayerHealth)
                        statistics.highestPlayerHealth = (int) value;
                    //
                    // var playerHealth = achievementSave.achievements.First(x=>x.id == AchievementName.Reach100PlayerHealth);
                    // ProcessStatAchievement(playerHealth, value, 100);
                    break;
                case StatType.MoveSpeed:
                    if (value > statistics.highestPlayerSpeed)
                        statistics.highestPlayerSpeed = (int) value;
                    // var playerSpeed = achievementSave.achievements.First(x=>x.id == AchievementName.Reach10PlayerSpeed);
                    // ProcessStatAchievement(playerSpeed, value, 10);
                    break;
                case StatType.MeleeDamage:
                    if (value > statistics.highestSwordDamage)
                        statistics.highestSwordDamage = (int) value;
                    // var swordDamage = achievementSave.achievements.First(x=>x.id == AchievementName.Reach50SwordDamage);
                    // ProcessStatAchievement(swordDamage, value, 50);
                    break;
                case StatType.MeleeRange:
                    if (value > statistics.highestSwordRange)
                        statistics.highestSwordRange = (int) value;
                    // var swordRange = achievementSave.achievements.First(x=>x.id == AchievementName.Reach10SwordRange);
                    // ProcessStatAchievement(swordRange, value, 10);
                    break;
                case StatType.MeleeAttackSpeed:
                    if (value > statistics.highestSwordAttackSpeed)
                        statistics.highestSwordAttackSpeed = (int) value;
                    // var swordAttackSpeed = achievementSave.achievements.First(x=>x.id == AchievementName.Reach8SwordAttackSpeed);
                    // ProcessStatAchievement(swordAttackSpeed, value, 8);
                    break;
                case StatType.MeleeKnockBack:
                    if (value > statistics.highestSwordKnockBack)
                        statistics.highestSwordKnockBack = (int) value;
                    // var swordKnockBack = achievementSave.achievements.First(x=>x.id == AchievementName.Reach10SwordKnockBack);
                    // ProcessStatAchievement(swordKnockBack, value, 10);
                    break;
                case StatType.HealthPackDropRate:
                    if (value > statistics.highestHealthPackSpawnRate)
                        statistics.highestHealthPackSpawnRate = (int) value;
                    // var healthPackSpawnRate = achievementSave.achievements.First(x=>x.id == AchievementName.Reach10HealthPackSpawnRate);
                    // ProcessStatAchievement(healthPackSpawnRate, value, 10);
                    break;
                case StatType.Luck:
                    if (value > statistics.highestLuck)
                        statistics.highestLuck = (int) value;
                    // var luck = achievementSave.achievements.First(x=>x.id == AchievementName.Reach7Luck);
                    // ProcessStatAchievement(luck, value, 7);
                    break;
                case StatType.Block:
                    if (value > statistics.highestBlock)
                        statistics.highestBlock = (int) value;
                    // var block = achievementSave.achievements.First(x=>x.id == AchievementName.Reach5Block);
                    // ProcessStatAchievement(block, value, 5);
                    break;
            }
        }

        public void AchievementUnlocked(Achievement achievement)
        {
            achievement.isCompleted = true;
            StartCoroutine(ShowAchievementPopup(achievement));
        }

        public void AchievementClaimed(Achievement achievement)
        {
            achievement.isClaimed = true;
            gold.AddGold(achievement.rewardGold);
        }
        
        private IEnumerator ShowAchievementPopup(Achievement achievement)
        {
            achievementPopup.SetActive(true);
            achievementPopupText.text = $"Achievement Unlocked:\n{achievement.uiName}";
            yield return new WaitForSeconds(3f);
            achievementPopup.SetActive(false);
        }

        private void OnValidate()
        {
            if (gold == null)
            {
                gold = FindObjectsByType<Gold>(FindObjectsSortMode.None).First();
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
    
    public int highestBlock;
}

[Serializable]
public class AchievementSave
{
    public Achievement[] achievements;
}

[Serializable]
public class SettingsSave
{
    public float musicVolume = 0.25f;
    public float sfxVolume = 0.5f;
    public bool showDamageNumbers = true;
    public bool showEnemyHealthBars = true;
    public bool isHyperMode = false;
}



public enum AchievementName
{
    Kill100Enemies,
    Kill1000Enemies,
    Kill10000Enemies,
    Kill100000Enemies,
    Kill100Bosses,
    Kill1000Bosses,
    Die,
    Die50Times,
    Die100Times,
    Open100Chests,
    Open1000Chests,
    Earn100Gold,
    Earn1000Gold,
    Earn10000Gold,
    Earn100000Gold,
    BeatTheGame,
    BeatTheGame10Times,
    WinInUnder1Hour,
    WinInUnder45Minutes,
    WinInUnder30Minutes,
    BuyAllStoreItems,
    Reach50PistolDamage,
    Reach25PistolRange,
    Reach15PistolFireRate,
    Reach10PistolKnockBack,
    Reach3PistolPierce,
    Reach100PlayerHealth,
    Reach10PlayerSpeed,
    Reach50SwordDamage,
    Reach10SwordRange,
    Reach8SwordAttackSpeed,
    Reach10SwordKnockBack,
    Reach540SwordArc,
    Reach10HealthPackSpawnRate,
    Reach7Luck,
    Reach5Block,
    Reach60Dodge,
    BeatRound1,
    BeatRound2,
    BeatRound3,
    BeatRound4,
    BeatRound5
}
