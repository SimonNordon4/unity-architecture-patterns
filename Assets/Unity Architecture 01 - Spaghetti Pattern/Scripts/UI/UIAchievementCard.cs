using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIAchievementCard : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI progressText;
        public Image progressBar;
        public Button collectButton;
        public TextMeshProUGUI collectButtonText;
        public UIAchievementMenu parent;

        public void Initialize(Achievement achievement, UIAchievementMenu parent)
        {
            this.parent = parent;
            titleText.text = achievement.uiName;

            var progress = Mathf.Clamp(achievement.progress, 0, achievement.goal);
            progressText.text = $"{progress}/{achievement.goal}";
            var progressScale = Mathf.Clamp((achievement.progress / (float)achievement.goal), 0, 1);
            progressBar.GetComponent<Image>().fillAmount = progressScale;
        }
    }
}