using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIAchievementMenu : MonoBehaviour
    {
        public RectTransform achievementEntryContainer;
        public UIAchievementEntry achievementEntryUIGameObject;
        public List<UIAchievementEntry> achievementEntryUis = new List<UIAchievementEntry>();
        public TextMeshProUGUI totalAchievementText;

        private void OnEnable()
        {
            Init();
        }

        public void UpdateAchievements()
        {
            Clear();
            Init();
        }

        void Init()
        {
            var achievements = AccountManager.Instance.achievementSave.achievements.ToList();

            // sort achievements by completed (incomplete first)
            achievements = achievements.OrderBy(x => x.isCompleted).ToList();

            for(int i = 0; i < achievements.Count; i++)
            {
                // Skip the first one because it already exists!
                if (i == 0)
                    continue;   
                var achievementUI = Instantiate(achievementEntryUIGameObject, achievementEntryContainer);
                achievementUI.Initialize(achievements[i], this);
                achievementUI.parent = this;
                achievementEntryUis.Add(achievementUI);
            }

            achievementEntryUIGameObject.Initialize(achievements[0], this);

            var completedAchievements = AccountManager.Instance.achievementSave.achievements
                .Sum(x => x.isCompleted ? 1 : 0);

            var totalAchievements = AccountManager.Instance.achievementSave.achievements.Length;
            totalAchievementText.text = $"Earned: {completedAchievements}/{totalAchievements}";
        }

        void Clear()
        {
            // Destroy all the store item uis.
            foreach (var storeItemUi in achievementEntryUis)
            {
                if (storeItemUi != null)
                    Destroy(storeItemUi.gameObject);
            }
            achievementEntryUis.Clear();
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}