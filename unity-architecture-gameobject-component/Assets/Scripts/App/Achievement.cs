using System;
using UnityEngine;

namespace GameObjectComponent.App
{
    [Serializable]
    public class Achievement
    {
        public Achievement()
        {

        }
        
        public Achievement(AchievementDefinition def)
        {
            id = def.id;
            uiName = def.uiName;
            rewardGold = def.rewardGold;
            isCompleted = false;
            progress = 0;
            goal = def.goal;
            isClaimed = false;
            statType = def.statType;
        }
        
        public AchievementId id;
        public string uiName;
        public int rewardGold;
        public bool isCompleted;
        public int progress;
        public int goal;
        public bool isClaimed;
        public StatType statType;
    }

    public enum AchievementId
    {
        KillEnemies,
        KillBosses,
        PlayerDied,
        OpenChests,
        EarnGold,
        WinGame,
        ReachStat,
        WinRound,
        WinInUnderMinutes,
    }
}