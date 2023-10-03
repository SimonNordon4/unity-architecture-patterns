using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public Image progressBar;
    public Button collectButton;
    public TextMeshProUGUI collectButtonText;
    public AchievementMenuManager parent;
    
    public void Initialize(Achievement achievement, AchievementMenuManager parent)
    {
        this.parent = parent;
        titleText.text = achievement.uiName;
        
        var progress = Mathf.Clamp(achievement.progress,0,achievement.goal);
        progressText.text = $"{progress}/{achievement.goal}";
        var progressScale = Mathf.Clamp((achievement.progress / (float) achievement.goal), 0, 1);
        progressBar.GetComponent<Image>().fillAmount = progressScale;
        
        if (achievement.isClaimed)
        {
            Collected();
        }

        Debug.Log("Completed? " + achievement.isCompleted + " Claimed? " + achievement.isClaimed);
        if (achievement.isCompleted && !achievement.isClaimed)
        {
            collectButton.interactable = true;
            collectButtonText.text = $"Collect {achievement.rewardGold}G";
        }
        else if(achievement.isClaimed)
        {
            collectButton.interactable = false;
            collectButtonText.text = $"Claimed";
        }
        else
        {
            collectButton.interactable = false;
            collectButtonText.text = $"Locked";
        }
        
        collectButton.onClick.AddListener(() =>
        {
            AccountManager.instance.AchievementClaimed(achievement);
            this.Collected();
        });
    }

    public void Collected()
    {
        collectButtonText.text = "Claimed";
        collectButton.interactable = false;
    }
}
