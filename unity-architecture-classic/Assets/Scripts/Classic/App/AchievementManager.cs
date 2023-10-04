using System.Collections.Generic;
using Classic.Game;
using Classic.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.App
{
    public class AchievementManager : MonoBehaviour
    {
        [Header("Dependencies")] 
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameState state;
        [SerializeField] private RoundTimer roundTimer;
        [SerializeField] private Gold gold;
        [SerializeField] private Stats stats;
        
        public UnityEvent<Achievement> onAchievementCompleted = new();

        private void OnEnable()
        {
            state.onGameWon.AddListener(GameWon);
            state.onGameWon.AddListener(GameWonInMinutes);
            state.onGameLost.AddListener(PlayerDied);
            enemyManager.onEnemyDied.AddListener(vec => EnemyDied());
            enemyManager.onBossDied.AddListener(vec => BossDied());
            Chest.OnChestPickedUp.AddListener(chest => ChestOpened());
            gold.onGoldChanged.AddListener(i => EarnedGold());
            stats.onStatChanged.AddListener(StatCheck);
            
            Load();
        }


        private void OnDisable()
        {
            state.onGameWon.RemoveListener(GameWon);
            state.onGameWon.RemoveListener(GameWonInMinutes);
            state.onGameLost.RemoveListener(PlayerDied);
            enemyManager.onEnemyDied.RemoveListener(vec => EnemyDied());
            enemyManager.onBossDied.RemoveListener(vec => BossDied());
            Chest.OnChestPickedUp.RemoveListener(chest => ChestOpened());
            gold.onGoldChanged.RemoveListener(i => EarnedGold());
            stats.onStatChanged.RemoveListener(StatCheck);
            
            Save();
        }

        [SerializeField] private List<AchievementDefinition> achievementDefinitions = new();
        [field:SerializeField] public Achievement[] achievements { get; private set; }
        
        private void CreateAchievements()
        {
            achievements = new Achievement[achievementDefinitions.Count];
            for (var i = 0; i < achievementDefinitions.Count; i++)
            {
                achievements[i] = new Achievement(achievementDefinitions[i]);
            }
        }
        private void Start()
        {
            Load();
        }

        private void Save()
        {
            var save = new AchievementSave(achievements);
            var json = JsonUtility.ToJson(save);
            PlayerPrefs.SetString("Achievements", json);
        }
        
        private void Load()
        {
            var json = PlayerPrefs.GetString("Achievements", null);
            var save = JsonUtility.FromJson<AchievementSave>(json);

            if(save == null)
            {
                CreateAchievements();
                Save();
                return;
            }
            
            achievements = save.savedAchievements;
        }
        public void ResetAll()
        {
            CreateAchievements();
            Save();
        }
        
        public void SetDefinitions(List<AchievementDefinition> definitions)
        {
            achievementDefinitions = definitions;
        }

        #region Achievement Checks
        
        public void EnemyDied()
        {
            CheckIncrementalAchievement(AchievementId.KillEnemies);
        }
        public void BossDied()
        {
            CheckIncrementalAchievement(AchievementId.KillBosses);
        }

        public void PlayerDied()
        {
            CheckIncrementalAchievement(AchievementId.PlayerDied);
        }
        
        public void ChestOpened()
        {
            CheckIncrementalAchievement(AchievementId.OpenChests);
        }

        public void GameWon()
        {
            CheckIncrementalAchievement(AchievementId.WinGame);
        }
        
        public void GameWonInMinutes()
        {
            foreach(var achievement in achievements)
            {
                if(achievement.id != AchievementId.WinInUnderMinutes) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;
                if (roundTimer.roundTime <= achievement.goal)
                {
                   CompleteAchievement(achievement);
                }
            }
        }



        public void StatCheck(StatType statType)
        {
            foreach (var achievement in achievements)
            {
                if (achievement.id != AchievementId.ReachStat) continue;
                if (achievement.statType != statType) continue;
                if (achievement.isCompleted) continue;
                if (achievement.isClaimed) continue;
                
                var statValue = stats.statMap[statType].value;
                achievement.progress = (int)(statValue > achievement.progress ? statValue : achievement.progress);
                if (achievement.progress >= achievement.goal)
                {
                    CompleteAchievement(achievement);
                }
            }
        }
        
        public void EarnedGold()
        {
            foreach (var achievement in achievements)
            {
                if(achievement.id != AchievementId.EarnGold) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;
                
                achievement.progress = gold.totalEarned;
                if (achievement.progress >= achievement.goal)
                {
                    CompleteAchievement(achievement);
                }
            }
        }
        #endregion
        
        private void CompleteAchievement(Achievement achievement)
        {
            achievement.isCompleted = true;
            onAchievementCompleted.Invoke(achievement);
            Debug.Log($"Achievement Completed: {achievement.uiName}");
        }
        
        private void CheckIncrementalAchievement(AchievementId id)
        {
            foreach (var achievement in achievements)
            {
                if(achievement.id != id) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;

                achievement.progress++;
                if (achievement.progress >= achievement.goal)
                {
                    CompleteAchievement(achievement);
                }
            }
        }
    }
}