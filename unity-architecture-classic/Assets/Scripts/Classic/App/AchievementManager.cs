using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.App
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField] private List<AchievementDefinition> achievementDefinitions = new();
        public Achievement[] achievements { get; private set; }
        
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
            foreach (var achievement in achievements)
            {
                if(achievement.id != AchievementId.KillEnemies) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;

                achievement.progress++;
                if (achievement.progress >= achievement.goal)
                {
                    achievement.isCompleted = true;
                }
            }
        }

        public void BossDied()
        {
            foreach (var achievement in achievements)
            {
                if(achievement.id != AchievementId.KillBosses) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;

                achievement.progress++;
                if (achievement.progress >= achievement.goal)
                {
                    achievement.isCompleted = true;
                }
            }
        }

        public void PlayerDied()
        {
            foreach (var achievement in achievements)
            {
                if(achievement.id != AchievementId.PlayerDied) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;

                achievement.progress++;
                if (achievement.progress >= achievement.goal)
                {
                    achievement.isCompleted = true;
                }
            }
        }
        
        public void ChestOpened()
        {
            foreach (var achievement in achievements)
            {
                if(achievement.id != AchievementId.OpenChests) continue;
                if(achievement.isCompleted) continue;
                if(achievement.isClaimed) continue;

                achievement.progress++;
                if (achievement.progress >= achievement.goal)
                {
                    achievement.isCompleted = true;
                }
            }
        }

        public void GameWon()
        {

        }

        public void StatCheck()
        {
            
        }
        #endregion
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var achievementManager = (AchievementManager) target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Reset All"))
            {
                achievementManager.ResetAll();
            }

            if (GUILayout.Button("Get Achievement Definitions"))
            {
                // find all achievement definitions in the project and add them to the list.
                var achievementDefinitions = new List<AchievementDefinition>();
                var guids = AssetDatabase.FindAssets("t:AchievementDefinition");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<AchievementDefinition>(path);
                    achievementDefinitions.Add(asset);
                    achievementManager.SetDefinitions(achievementDefinitions);
                }
                
            }
        }
    }
    #endif
}