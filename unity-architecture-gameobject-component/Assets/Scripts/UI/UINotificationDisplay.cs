using System.Linq;
using GameObjectComponent.App;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    [DefaultExecutionOrder(100)]
    public class UINotificationDisplay : MonoBehaviour
    {
        [SerializeField]private AchievementManager achievementManager;
        [SerializeField]private Store store;
        [SerializeField] private Gold gold;
        
        [SerializeField]private GameObject achievementNotification;
        [SerializeField]private GameObject storeNotification;
        
        private TextMeshProUGUI _achievementText;

        private void Awake()
        {
            _achievementText = achievementNotification.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            achievementNotification.SetActive(false);
            storeNotification.SetActive(false);
            CalculateAchievementNotification();
            CalculateStoreNotification();
        }

        private void CalculateAchievementNotification()
        {
            // look at how many achievements are completed but not claimed
            var completedAchievements = achievementManager.achievements.Count(achievement => achievement.isCompleted && !achievement.isClaimed);
            // set the text
            _achievementText.text = completedAchievements.ToString();
            achievementNotification.SetActive(completedAchievements > 0);

        }
        
        private void CalculateStoreNotification()
        {
            Debug.Log("Calculating store notification");
            // Check if there's any affordable store items
            foreach (var storeItem in store.purchasedStoreItems)
            {
                Debug.Log($"Checking {storeItem.storeName} upgrade {storeItem.upgrades[storeItem.currentUpgrade].cost} is less than {gold.amount}");

                if (storeItem.upgrades[storeItem.currentUpgrade].cost > gold.amount) continue;
                storeNotification.SetActive(true);
                return;
            }
            
            
        }
    }
}