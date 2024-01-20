using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Game;
using GameObjectComponent.Items;
using GameplayComponents.Actor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectComponent.App
{
    public class AchievementManager : PersistentComponent
    {
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private GameState state;
        [SerializeField] private RoundTimer roundTimer;
        [SerializeField] private Gold gold;
        [SerializeField] private Stats stats;
        
        public UnityEvent<Achievement> onAchievementCompleted = new();
        
        [SerializeField] private List<AchievementDefinition> achievementDefinitions = new();
        [field:SerializeField] public Achievement[] achievements { get; private set; }

        private void OnEnable()
        {
            state.OnGameWon+=(GameWon);
            state.OnGameWon+=(GameWonInMinutes);
            state.OnGameLost+=(PlayerDied);
            waveSpawner.onWaveActorDied.AddListener(vec => WaveActorDied());
            waveSpawner.OnWaveCompleted += vec3 => WaveCompleted();
            Chest.OnChestPickedUp += chest => ChestOpened();
            gold.OnGoldChanged += (i => EarnedGold());
            stats.onStatChanged += stat => StatCheck(stat.type);
            
            Load();
        }


        private void OnDisable()
        {
            Save();
        }
        
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

        public override void Save()
        {
            var save = new AchievementSave(achievements);
            var json = JsonUtility.ToJson(save);
            PlayerPrefs.SetString("Achievements", json);
        }
        
        public override void Load()
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
        public override void Reset()
        {
            CreateAchievements();
            Save();
        }
        
        public void SetDefinitions(List<AchievementDefinition> definitions)
        {
            achievementDefinitions = definitions;
        }

        #region Achievement Checks
        
        public void WaveActorDied()
        {
            CheckIncrementalAchievement(AchievementId.KillEnemies);
        }
        public void WaveCompleted()
        {
            CheckIncrementalAchievement(AchievementId.WaveCompleted);
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
                
                var statValue = stats.GetStat(statType).value;
                
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
            achievement.progress = achievement.goal;
            Debug.Log($"Achievement Unlocked: {achievement.uiName}");
            onAchievementCompleted.Invoke(achievement);
            
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
        
        #if UNITY_EDITOR
        [ContextMenu("Find Achievement Definitions")]
        public void FindAchievementDefinitions()
        {
            achievementDefinitions = AssetDatabase.FindAssets("t:AchievementDefinition")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AchievementDefinition>)
                .ToList();
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        #endif
    }
}