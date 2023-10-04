using System.Linq;
using UnityEngine;

namespace Classic.App
{
    [CreateAssetMenu(fileName = "AchievementDefinition", menuName = "Classic/AchievementDefinition")]
    public class AchievementDefinition : ScriptableObject
    {
        public AchievementId id;
        public string uiName;
        public int goal;
        public int rewardGold;
        // only relevant for stat achievements.
        public StatType statType;
    }
}