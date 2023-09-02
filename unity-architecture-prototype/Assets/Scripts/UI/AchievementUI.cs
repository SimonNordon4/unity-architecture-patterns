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
        progressText.text = $"{achievement.progress}/{achievement.goal}";
        progressBar.transform.localScale = new Vector3(achievement.progress / (float) achievement.goal, 1, 1);
        collectButton.interactable = achievement.isCompleted;

        if (achievement.isClaimed)
        {
            Collected();
        }
        
        collectButtonText.text = achievement.isCompleted ? $"Collect {achievement.rewardGold}G" : "Locked";
        
        collectButton.onClick.AddListener(() =>
        {
            AccountManager.instance.AchievementClaimed(achievement);
            this.Collected();
        });
    }

    public void Collected()
    {
        if(parent!=null)
            parent.UpdateGoldText();
        
        collectButton.interactable = false;
        collectButtonText.text = "Claimed";
        collectButton.colors = new ColorBlock()
        {
            normalColor = Color.gray,
            highlightedColor = Color.gray,
            pressedColor = Color.gray,
            selectedColor = Color.gray,
            disabledColor = new Color(0.5f,0.75f,0.5f)
        };
    }
}
