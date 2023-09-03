using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    public GameObject achievementNotification;
    public TextMeshProUGUI achievementText;

    public GameObject storeNotification;
    public TextMeshProUGUI storeText;

    private void OnEnable()
    {
        CalculateNotifications();
    }

    private void CalculateNotifications()
    {
        // Get all achievements that are completed but not collected.
        int numberOfCompletedAchievements = 0;

        var achievementCount = AccountManager.instance.achievementSave.achievements.Length;
        
        for(var i = 0; i < achievementCount; i++)
        {
            var achievement = AccountManager.instance.achievementSave.achievements[i];
            if (achievement.isCompleted && !achievement.isClaimed)
            {
                numberOfCompletedAchievements++;
            }
        }

       
        Debug.Log($"Number of completed achievements: {numberOfCompletedAchievements}");
        if (numberOfCompletedAchievements > 0)
        {
            achievementNotification.SetActive(true);
            achievementText.text = numberOfCompletedAchievements.ToString();
        }
        else
        {
            achievementNotification.SetActive(false);
        }
        
        // Check how many store items can be bought
        int numberOfStoreItems = 0;
        foreach (var storeItem in AccountManager.instance.storeItems)
        {
            if(storeItem.currentTier >= storeItem.pricePerTier.Length) continue;
            
            if (storeItem.pricePerTier[storeItem.currentTier] <= AccountManager.instance.totalGold)
            {
                numberOfStoreItems++;
            }
        }
        
        if (numberOfStoreItems > 0)
        {
            storeNotification.SetActive(true);
            storeText.text = numberOfStoreItems.ToString();
        }
        else
        {
            storeNotification.SetActive(false);
        }
    }
}
