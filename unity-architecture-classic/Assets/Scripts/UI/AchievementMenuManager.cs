using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Classic.App;
using Classic.Utility;

public class AchievementMenuManager : MonoBehaviour
{
    [SerializeField]private AchievementManager achievementManager;
    
    public RectTransform achievementItemContainer;
    public AchievementUI achievementItemUIPrefab;
    public List<AchievementUI> achievementItemUis = new List<AchievementUI>();
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

    
    void Init()
    {
        // Populate all the store item uis.
        foreach (var achievement in achievementManager.achievements)
        {
            var achievementUI = Instantiate(achievementItemUIPrefab, achievementItemContainer);
            achievementUI.Initialize(achievement,this);
            achievementUI.parent = this;
            achievementItemUis.Add(achievementUI);
        }
        
        var completedAchievements = achievementManager.achievements
            .Sum(x => x.isCompleted ? 1 : 0);
        
        var totalAchievements = achievementManager.achievements.Length;
        totalAchievementText.text =$"Earned: {completedAchievements.ToString()}/{totalAchievements.ToString()}";
    }

    void Clear()
    {
        // Destroy all the store item uis.
        foreach (var storeItemUi in achievementItemUis)
        {
            if(storeItemUi != null)
                Destroy(storeItemUi.gameObject);
        }
        achievementItemUis.Clear();
    }
    
    private void OnDisable()
    {
        Clear();
    }

    private void OnValidate()
    {
        achievementManager = SurvivorsUtil.Find<AchievementManager>();
    }
}
