using System;

namespace GameObjectComponent.App
{
    [Serializable]
    public class AchievementSave
    {
        public AchievementSave(Achievement[] achievements)
        {
            savedAchievements = achievements;
        }
        public Achievement[] savedAchievements;
    }
}