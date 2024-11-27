using UnityEngine;

namespace GameObjectComponent.App
{
    public class AchievementPopupHandler : MonoBehaviour
    {
        [SerializeField]private AchievementManager achievementManager;
        [SerializeField]private PopUpScheduler popUpScheduler;

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
            popUpScheduler.SchedulePopup($"Achievement Unlocked: {achievement.uiName}");
        }
    }
}