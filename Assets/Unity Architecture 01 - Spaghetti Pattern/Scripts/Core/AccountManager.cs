﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [DefaultExecutionOrder(-10)]
    public class AccountManager : MonoBehaviour
    {
        private static AccountManager _instance;
        public static AccountManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<AccountManager>();
                if (_instance == null)
                    _instance = new GameObject("AccountManager").AddComponent<AccountManager>();
                return _instance;
            }
            private set => _instance = value;
        }

        [Header("Statistics")]
        public StatisticsSave statistics = new();

        [Header("Achievements")]
        public AchievementSave achievementSave = new();

        [Header("Settings")]
        public SettingsSave settingsSave = new();


        public bool debugSkipLoad = false;

        private void Awake()
        {
            if(_instance != null)
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            CreateAchievements();
            Load();

        }
        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            var accountSave = new AccountSave();

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
            
            var json = PlayerPrefs.GetString("statistics");
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

            statistics = new StatisticsSave();
            CreateAchievements();
        }

        private void CreateAchievements()
        {
            Debug.Log("Creating achievements");
            var achievements = new List<Achievement>
            {
                new Achievement()
                {
                    name = AchievementName.Die,
                    uiName = "Die",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.Kill100Enemies,
                    uiName = "Kill 100 Enemies",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Kill1000Enemies,
                    uiName = "Kill 1000 Enemies",
                    goal = 1000,
                },
                new Achievement()
                {
                    name = AchievementName.Kill10000Enemies,
                    uiName = "Kill 10000 Enemies",
                    goal = 10000,
                },
                new Achievement()
                {
                    name = AchievementName.Kill100000Enemies,
                    uiName = "Kill 100000 Enemies",
                    goal = 100000,
                },
                new Achievement()
                {
                    name = AchievementName.Kill100Bosses,
                    uiName = "Kill 100 Bosses",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Kill1000Bosses,
                    uiName = "Kill 1000 Bosses",
                    goal = 1000,
                },
                new Achievement()
                {
                    name = AchievementName.Die50Times,
                    uiName = "Die 50 Times",
                    goal = 50,
                },
                new Achievement()
                {
                    name = AchievementName.Die100Times,
                    uiName = "Die 100 Times",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Open100Chests,
                    uiName = "Open 100 Chests",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Open1000Chests,
                    uiName = "Open 1000 Chests",
                    goal = 1000,
                },
                new Achievement()
                {
                    name = AchievementName.Earn100Gold,
                    uiName = "Earn 100 Gold",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Earn1000Gold,
                    uiName = "Earn 1000 Gold",
                    goal = 1000,
                },
                new Achievement()
                {
                    name = AchievementName.Earn10000Gold,
                    uiName = "Earn 10000 Gold",
                    goal = 10000,
                },
                new Achievement()
                {
                    name = AchievementName.Earn100000Gold,
                    uiName = "Earn 100000 Gold",
                    goal = 100000,
                },
                new Achievement()
                {
                    name = AchievementName.BeatTheGame,
                    uiName = "Beat The Game",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.BeatTheGame10Times,
                    uiName = "Beat The Game 10 Times",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.WinInUnder1Hour,
                    uiName = "Win In Under 1 Hour",
                    goal = 3600,
                },
                new Achievement()
                {
                    name = AchievementName.WinInUnder45Minutes,
                    uiName = "Win In Under 45 Minutes",
                    goal = 2700,
                },
                new Achievement()
                {
                    name = AchievementName.WinInUnder30Minutes,
                    uiName = "Win In Under 30 Minutes",
                    goal = 1800,
                },
                new Achievement
                {
                    name = AchievementName.Reach50PistolDamage,
                    uiName = "Reach 50 Pistol Damage",
                    goal = 50,
                },
                new Achievement()
                {
                    name = AchievementName.Reach25PistolRange,
                    uiName = "Reach 25 Pistol Range",
                    goal = 25,
                },
                new Achievement()
                {
                    name = AchievementName.Reach15PistolFireRate,
                    uiName = "Reach 15 Pistol Fire Rate",
                    goal = 15,
                },
                new Achievement()
                {
                    name = AchievementName.Reach10PistolKnockBack,
                    uiName = "Reach 10 Pistol Knock Back",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach3PistolPierce,
                    uiName = "Reach 3 Pistol Pierce",
                    goal = 3,
                },
                new Achievement()
                {
                    name = AchievementName.Reach100PlayerHealth,
                    uiName = "Reach 100 Player Health",
                    goal = 100,
                },
                new Achievement()
                {
                    name = AchievementName.Reach10PlayerSpeed,
                    uiName = "Reach 10 Player Speed",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach50SwordDamage,
                    uiName = "Reach 50 Sword Damage",
                    goal = 50,
                },
                new Achievement()
                {
                    name = AchievementName.Reach10SwordRange,
                    uiName = "Reach 10 Sword Range",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach8SwordAttackSpeed,
                    uiName = "Reach 8Sword Attack Speed",
                    goal = 8,
                },
                new Achievement()
                {
                    name = AchievementName.Reach10SwordKnockBack,
                    uiName = "Reach 10 Sword Knock Back",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach540SwordArc,
                    uiName = "Reach 540 Sword Arc",
                    goal = 540,
                },
                new Achievement()
                {
                    name = AchievementName.Reach10HealthPackSpawnRate,
                    uiName = "Reach 10 Health Pack Spawn Rate",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach7Luck,
                    uiName = "Reach 7 Luck",
                    goal = 10,
                },
                new Achievement()
                {
                    name = AchievementName.Reach5Block,
                    uiName = "Reach 5 Block",
                    goal = 5,
                },
                new Achievement()
                {
                    name = AchievementName.Reach60Dodge,
                    uiName = "Reach 60 Dodge",
                    goal = 60,
                },
                new Achievement()
                {
                    name = AchievementName.BeatRound1,
                    uiName = "Beat Round 1",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.BeatRound2,
                    uiName = "Beat Round 2",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.BeatRound3,
                    uiName = "Beat Round 3",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.BeatRound4,
                    uiName = "Beat Round 4",
                    goal = 1,
                },
                new Achievement()
                {
                    name = AchievementName.BeatRound5,
                    uiName = "Beat Round 5",
                    goal = 1,
                },
            };

            achievementSave = new AchievementSave
            {
                achievements = achievements.ToArray()
            };
        }

        private void ProcessStatAchievement(Achievement achievement, float value, int target)
        {
            if (value > achievement.progress)
                achievement.progress = Mathf.Clamp((int)value, 0, achievement.goal);
            if (value >= target)
                AchievementUnlocked(achievement);
        }

        // We can also add our own custom achievements here.
        public void CheckIfHighestStat(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Damage:
                    if (value > statistics.highestPistolDamage)
                        statistics.highestPistolDamage = (int)value;
                    var pistolDamage = achievementSave.achievements.First(x => x.name == AchievementName.Reach50PistolDamage);
                    ProcessStatAchievement(pistolDamage, value, 50);
                    break;
                case StatType.Range:
                    if (value > statistics.highestPistolRange)
                        statistics.highestPistolRange = (int)value;
                    var pistolRange = achievementSave.achievements.First(x => x.name == AchievementName.Reach25PistolRange);
                    ProcessStatAchievement(pistolRange, value, 25);
                    break;
                case StatType.FireRate:
                    if (value > statistics.highestPistolFireRate)
                        statistics.highestPistolFireRate = (int)value;
                    ProcessStatAchievement(achievementSave.achievements.First(x => x.name == AchievementName.Reach15PistolFireRate), value, 15);
                    break;
                case StatType.KnockBack:
                    if (value > statistics.highestPistolKnockBack)
                        statistics.highestPistolKnockBack = (int)value;
                    var pistolKnockBack = achievementSave.achievements.First(x => x.name == AchievementName.Reach10PistolKnockBack);
                    ProcessStatAchievement(pistolKnockBack, value, 10);

                    break;
                case StatType.Pierce:
                    if (value > statistics.highestPistolPierce)
                        statistics.highestPistolPierce = (int)value;
                    var pistolPierce = achievementSave.achievements.First(x => x.name == AchievementName.Reach3PistolPierce);
                    ProcessStatAchievement(pistolPierce, value, 3);
                    break;
                case StatType.MaxHealth:
                    if (value > statistics.highestPlayerHealth)
                        statistics.highestPlayerHealth = (int)value;

                    var playerHealth = achievementSave.achievements.First(x => x.name == AchievementName.Reach100PlayerHealth);
                    ProcessStatAchievement(playerHealth, value, 100);
                    break;
                case StatType.Speed:
                    if (value > statistics.highestPlayerSpeed)
                        statistics.highestPlayerSpeed = (int)value;
                    var playerSpeed = achievementSave.achievements.First(x => x.name == AchievementName.Reach10PlayerSpeed);
                    ProcessStatAchievement(playerSpeed, value, 10);
                    break;
            }
        }

        public void AchievementUnlocked(Achievement achievement)
        {
            achievement.isCompleted = true;
        }
    }


    [Serializable]
    public struct AccountSave
    {
        public int totalGold;
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

    [Serializable]
    public class Achievement
    {
        public AchievementName name;
        public string uiName;
        public bool isCompleted;
        public int progress;
        public int goal;
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
}