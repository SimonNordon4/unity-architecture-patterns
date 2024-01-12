using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using GameObjectComponent.App;
using GameObjectComponent.UI;
using GameObjectComponent.Utility;

public class UIAchievementMenu : MonoBehaviour
{
    [SerializeField]private AchievementManager achievementManager;
    [SerializeField] private Gold gold;
    
    [SerializeField]private RectTransform achievementItemContainer;
    [SerializeField]private UIAchievement achievementItemUIPrefab;
    private readonly List<UIAchievement> _achievementItemUis = new();
    [SerializeField]private TextMeshProUGUI totalAchievementText;
    
    private void OnEnable()
    {
        achievementManager.onAchievementCompleted.AddListener(a =>
        {
            Clear();
            Init();
        });
        Init();
    }
    
    
    void Init()
    {
        var sortedAchievements = achievementManager.achievements
            .OrderBy(x => x.isCompleted && !x.isClaimed ? 0 : !x.isCompleted ? 1 : 2)
            .ThenBy(x => x.isClaimed ? 1 : 0)
            .ToArray();
        
        // Populate all the store item uis.
        foreach (var achievement in sortedAchievements)
        {
            var achievementUI = Instantiate(achievementItemUIPrefab, achievementItemContainer);
            achievementUI.Construct(achievement, gold);
            achievementUI.Init();
            _achievementItemUis.Add(achievementUI);
        }
        
        var completedAchievements = achievementManager.achievements
            .Sum(x => x.isCompleted ? 1 : 0);
        
        var totalAchievements = achievementManager.achievements.Length;
        totalAchievementText.text =$"Earned: {completedAchievements.ToString()}/{totalAchievements.ToString()}";
    }

    void Clear()
    {
        // Destroy all the store item uis.
        foreach (var storeItemUi in _achievementItemUis)
        {
            if(storeItemUi != null)
                Destroy(storeItemUi.gameObject);
        }
        _achievementItemUis.Clear();
    }
    
    private void OnDisable()
    {
        Clear();
    }
}
