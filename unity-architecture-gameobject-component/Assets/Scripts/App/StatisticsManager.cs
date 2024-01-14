using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Game;

using GameObjectComponent.Items;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class StatisticsManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameState state;
        [SerializeField] private RoundTimer roundTimer;
        [SerializeField] private Gold gold;
        [SerializeField] private Stats stats;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private Health characterHealth;
        
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

        private void Awake()
        {
            Load();
        }

        private void OnEnable()
        {
            waveSpawner.onWaveActorDied.AddListener(EnemyDied);
            waveSpawner.OnWaveCompleted += (BossDied);
            state.OnGameWon+=(GameWon);
            state.OnGameLost+=(GameLost);
            state.OnGameQuit+=(GamePlayed);
            gold.OnGoldChanged+=(OnGoldChanged);
            stats.onStatChanged+=(OnStatChanged);
            Chest.OnChestPickedUp+=(OnChestPickedUp);
            characterHealth.OnHealthChanged+=(DamageTaken);
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

        private void OnStatChanged(Stat stat)
        {
            var highestStat = GetStatistic(StatisticType.HighestStat, stat.type);

            if (highestStat == null) return;
            
            if (highestStat.highestValue < (int)stat.value)
            {
                highestStat.highestValue = (int)stat.value;
            }
        }

        private void BossDied(Vector3 arg0)
        {
            var bossKills = GetStatistic(StatisticType.WavesCompleted);
            bossKills.highestValue++;
        }

        private void EnemyDied(Vector3 arg0)
        {
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

        [ContextMenu("Reset All")]
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

            Debug.LogWarning("No statistic found for type: " + statisticType + " and stat type: " + statType);
            return null;
        }

        public void SetDefinitions(List<StatisticDefinition> list)
        {
            statisticDefinitions = list;
            CreateStatistics();
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Find All Statistic Definitions")]
        public void FindAllStatisticDefinitions()
        {
            var definitions = UnityEditor.AssetDatabase.FindAssets("t:StatisticDefinition")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<StatisticDefinition>)
                .ToList();
            SetDefinitions(definitions);
        }
        #endif
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