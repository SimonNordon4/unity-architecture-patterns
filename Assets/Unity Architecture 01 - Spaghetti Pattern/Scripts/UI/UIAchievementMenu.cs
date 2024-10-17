using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIAchievementMenu : MonoBehaviour
    {
        public RectTransform achievementItemContainer;
        public UIAchievementCard achievementItemUIPrefab;
        public List<UIAchievementCard> achievementItemUis = new List<UIAchievementCard>();
        public TextMeshProUGUI totalAchievementText;
        public TextMeshProUGUI goldText;

        private void OnEnable()
        {
            Init();
        }

        public void UpdateStoreMenu()
        {
            Clear();
            Init();
        }

        public void UpdateGoldText()
        {
            goldText.text = $"GOLD: {AccountManager.instance.totalGold.ToString()}";
        }

        void Init()
        {
            // Populate all the store item uis.
            foreach (var achievement in AccountManager.instance.achievementSave.achievements)
            {
                var achievementUI = Instantiate(achievementItemUIPrefab, achievementItemContainer);
                achievementUI.Initialize(achievement, this);
                achievementUI.parent = this;
                achievementItemUis.Add(achievementUI);
            }

            var completedAchievements = AccountManager.instance.achievementSave.achievements
                .Sum(x => x.isCompleted ? 1 : 0);

            var totalAchievements = AccountManager.instance.achievementSave.achievements.Length;
            totalAchievementText.text = $"Earned: {completedAchievements.ToString()}/{totalAchievements.ToString()}";
            goldText.text = $"GOLD: {AccountManager.instance.totalGold.ToString()}";
        }

        void Clear()
        {
            // Destroy all the store item uis.
            foreach (var storeItemUi in achievementItemUis)
            {
                if (storeItemUi != null)
                    Destroy(storeItemUi.gameObject);
            }
            achievementItemUis.Clear();
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}