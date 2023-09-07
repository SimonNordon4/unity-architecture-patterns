using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[DefaultExecutionOrder(150)]
public class NotificationsManager : MonoBehaviour
{
    public GameObject achievementNotification;
    public TextMeshProUGUI achievementText;

    public GameObject storeNotification;
    public TextMeshProUGUI storeText;

    public bool highLightShop = false;
    public Color _defaultColor;

    public Button shopButton;
    

    private void OnEnable()
    {
        CalculateNotifications();
    }

    private void OnDisable()
    {
        if (achievementNotification != null)
        {
            achievementNotification.SetActive(false);
        }

        if (storeNotification != null)
        {
            storeNotification.SetActive(false);
        }
        
        shopButton.colors = new ColorBlock
        {
            normalColor =_defaultColor,
            highlightedColor = shopButton.colors.highlightedColor,
            pressedColor = shopButton.colors.pressedColor,
            selectedColor = shopButton.colors.selectedColor,
            disabledColor = shopButton.colors.disabledColor,
            colorMultiplier = 1
        };
    }

    private void CalculateNotifications()
    {
        if (achievementNotification != null)
        {
            // Get all achievements that are completed but not collected.
            int numberOfCompletedAchievements = 0;
            Debug.Log("Number of achievements: " + numberOfCompletedAchievements);
            

            var achievementCount = AccountManager.instance.achievementSave.achievements.Length;
        
            for(var i = 0; i < achievementCount; i++)
            {
                var achievement = AccountManager.instance.achievementSave.achievements[i];
                if (achievement.isCompleted && !achievement.isClaimed)
                {
                    numberOfCompletedAchievements++;
                }
            }

       
            if (numberOfCompletedAchievements > 0)
            {
                achievementNotification.SetActive(true);
                achievementText.text = numberOfCompletedAchievements.ToString();
            }
            else
            {
                achievementNotification.SetActive(false);
            }
        }

        if (storeNotification != null)
        {
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
            
            Debug.Log("Number of store items: " + numberOfStoreItems);
        
            if (numberOfStoreItems > 0)
            {
                storeNotification.SetActive(true);
                storeText.text = numberOfStoreItems.ToString();
                shopButton.colors = new ColorBlock
                {
                    normalColor = new Color(0.75f,1f,0.75f),
                    highlightedColor = shopButton.colors.highlightedColor,
                    pressedColor = shopButton.colors.pressedColor,
                    selectedColor = shopButton.colors.selectedColor,
                    disabledColor = shopButton.colors.disabledColor,
                    colorMultiplier = 1
                };
            }
            else
            {
                storeNotification.SetActive(false);
                shopButton.colors = new ColorBlock
                {
                    normalColor =_defaultColor,
                    highlightedColor = shopButton.colors.highlightedColor,
                    pressedColor = shopButton.colors.pressedColor,
                    selectedColor = shopButton.colors.selectedColor,
                    disabledColor = shopButton.colors.disabledColor,
                    colorMultiplier = 1
                };
            }
        }
    }
}
