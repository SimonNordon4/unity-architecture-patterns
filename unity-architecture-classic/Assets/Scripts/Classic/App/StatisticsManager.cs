using System;
using System.Collections.Generic;
using System.Linq;
using Classic.Character;
using Classic.Game;
using Classic.Items;
using UnityEngine;

namespace Classic.App
{
    public class StatisticsManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameState state;
        [SerializeField] private RoundTimer roundTimer;
        [SerializeField] private Gold gold;
        [SerializeField] private Stats stats;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private CharacterHealth characterHealth;
        
        [SerializeField] private List<StatisticDefinition> statisticDefinitions;
        [field:SerializeField] public Statistic[] statistics { get; private set; }

        public void CreateStatistics()
        {
            statistics = new Statistic[statisticDefinitions.Count];
            for (var i = 0; i < statisticDefinitions.Count; i++)
            {
                statistics[i] = new Statistic(statisticDefinitions[i]);
            }
        }

        private void OnEnable()
        {
            enemyManager.onEnemyDied.AddListener(EnemyDied);
            enemyManager.onBossDied.AddListener(BossDied);
            enemyManager.onDamageTaken.AddListener(DamageDealt);
            state.OnGameWon+=(GameWon);
            state.OnGameLost+=(GameLost);
            state.OnGameQuit+=(GamePlayed);
            gold.OnGoldChanged+=(OnGoldChanged);
            stats.onStatChanged.AddListener(OnStatChanged);
            Chest.OnChestPickedUp.AddListener(OnChestPickedUp);
            characterHealth.onDamaged.AddListener(DamageTaken);
            characterHealth.onHeal.AddListener(Healed);
            
            Load();
        }

        private void Healed(int healing)
        {
            var damageHealed = GetStatistic(StatisticType.DamageHealed);
            damageHealed.highestValue += healing;
        }

        private void DamageTaken(int damage)
        {
            var damageTaken = GetStatistic(StatisticType.DamageTaken);
            damageTaken.highestValue += damage;
        }

        private void DamageDealt(int damage)
        {
            var damageDealt = GetStatistic(StatisticType.DamageDealt);
            damageDealt.highestValue += damage;
        }

        private void GameLost()
        {
            var gamesPlayed = GetStatistic(StatisticType.GamesPlayed);
            gamesPlayed.highestValue++;
            
            var deaths = GetStatistic(StatisticType.Deaths);
            deaths.highestValue++;
        }

        private void OnChestPickedUp(Chest arg0)
        {
            var chestsOpened = GetStatistic(StatisticType.ChestsOpened);
            chestsOpened.highestValue++;
        }

        private void GamePlayed()
        {
            var gamesPlayed = GetStatistic(StatisticType.GamesPlayed);
            gamesPlayed.highestValue++;
        }

        private void OnStatChanged(StatType type)
        {
            var highestStat = GetStatistic(StatisticType.HighestStat, type);
            var stat = stats.statMap[type];
            if (highestStat.highestValue < (int)stat.value)
            {
                highestStat.highestValue = (int)stat.value;
            }
        }

        private void BossDied(Vector3 arg0)
        {
            var bossKills = GetStatistic(StatisticType.BossKills);
            bossKills.highestValue++;
        }

        private void EnemyDied(Vector3 arg0)
        {
            Debug.Log("Enemy Died");
            var totalKills = GetStatistic(StatisticType.TotalKills);
            totalKills.highestValue++;
        }

        private void OnGoldChanged(int newGold)
        {
            var goldEarned = GetStatistic(StatisticType.GoldEarned);
            if (goldEarned.highestValue < gold.totalEarned)
            {
                goldEarned.highestValue = gold.totalEarned;
            }
        }
        
        

        private void GameWon()
        {
            var gamesWon = GetStatistic(StatisticType.GamesWon);
            gamesWon.highestValue++;
            
            var fastestWin = GetStatistic(StatisticType.FastestWin);
            var roundTime = roundTimer.roundTime;
            
            if (fastestWin.highestValue == 0 || roundTime < fastestWin.highestValue)
            {
                fastestWin.highestValue = (int)roundTime;
            }
            
            var gamesPlayed = GetStatistic(StatisticType.GamesPlayed);
            gamesPlayed.highestValue++;
        }

        private void OnDisable()
        {
            Save();
        }

        private void Save()
        {
            var save = new StatisticsSave(statistics);
            var json = JsonUtility.ToJson(save);
            PlayerPrefs.SetString("Statistics", json);
        }
        
        private void Load()
        {
            var json = PlayerPrefs.GetString("Statistics", null);
            var save = JsonUtility.FromJson<StatisticsSave>(json);
            if(save == null)
            {
                CreateStatistics();
                Save();
                return;
            }
            statistics = save.statistics;
        }

        public void ResetAll()
        {
            CreateStatistics();
            Save();
        }

        private Statistic GetStatistic(StatisticType statisticType, StatType? statType = default)
        {
            foreach(var statistic in statistics)
            {
                if (statistic.statisticType != statisticType) continue;
                
                if (statisticType == StatisticType.HighestStat)
                {
                    if (statistic.statType == statType)
                    {
                        return statistic;
                    }
                }
                else
                {
                    return statistic;
                }
            }

            Debug.LogError("No statistic found for type: " + statisticType + " and stat type: " + statType);
            return null;
        }

        public void SetDefinitions(List<StatisticDefinition> list)
        {
            statisticDefinitions = list;
            CreateStatistics();
        }
    }
    
    [Serializable]
    public class StatisticsSave
    {
        public StatisticsSave(Statistic[] statistics)
        {
            this.statistics = statistics;
        }
        public Statistic[] statistics;
    }
}