using GameObjectComponent.App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIAchievement : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI titleText;
        [SerializeField]private TextMeshProUGUI progressText;
        [SerializeField]private Image progressBar;
        [SerializeField]private Button collectButton;
        [SerializeField]private TextMeshProUGUI collectButtonText;
        
        private Achievement _achievement;
        private Gold _gold;

        public void Construct(Achievement achievement, Gold gold)
        {
            _achievement = achievement;
            _gold = gold;
        }

        public void Init()
        {
            Reset();
            UpdateTitle();
            UpdateProgress();
            UpdateStatus();
            collectButton.onClick.AddListener(CollectAchievement);
        }

        private void Reset()
        {
            titleText.text = "";
            progressText.text = "";
            progressBar.fillAmount = 0;
            collectButtonText.text = "";
            collectButtonText.text = "";
            collectButton.onClick.RemoveAllListeners();
            collectButton.interactable = true;
        }

        private void UpdateTitle()
        {
            titleText.text = _achievement.uiName;
        }

        private void UpdateProgress()
        {
            var progress = Mathf.Clamp(_achievement.progress, 0, _achievement.goal);
            progressText.text = $"{progress}/{_achievement.goal}";
            var progressScale = Mathf.Clamp((_achievement.progress / (float)_achievement.goal), 0, 1);
            progressBar.GetComponent<Image>().fillAmount = progressScale;
        }

        private void UpdateStatus()
        {
            if (_achievement.isCompleted && !_achievement.isClaimed)
            {
                collectButton.interactable = true;
                collectButtonText.text = $"Collect {_achievement.rewardGold}G";
            }
            else if (_achievement.isClaimed)
            {
                collectButton.interactable = false;
                collectButtonText.text = $"Claimed";
            }
            else
            {
                collectButton.interactable = false;
                collectButtonText.text = $"Locked";
            }
        }
        
        public void CollectAchievement()
        {
            _achievement.isClaimed = true;
            _gold.ChangeGold(_achievement.rewardGold);
            UpdateStatus();
            
        }
    }
}