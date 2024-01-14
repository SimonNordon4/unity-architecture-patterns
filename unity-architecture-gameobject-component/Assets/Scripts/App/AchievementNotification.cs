using UnityEngine;

namespace GameObjectComponent.App
{
    public class AchievementNotification : MonoBehaviour
    {
        [SerializeField]private AchievementManager achievementManager;
        [SerializeField]private NotificationManager notificationManager;

        private void OnEnable()
        {
            achievementManager.onAchievementCompleted.AddListener(OnAchievementCompleted);
        }

        private void OnDisable()
        {
            achievementManager.onAchievementCompleted.RemoveListener(OnAchievementCompleted);
        }

        public void OnAchievementCompleted(Achievement achievement)
        {
            Debug.Log($"Achievement Unlocked: {achievement.uiName}");
            notificationManager.ScheduleNotification($"Achievement Unlocked: {achievement.uiName}");
        }
    }
}