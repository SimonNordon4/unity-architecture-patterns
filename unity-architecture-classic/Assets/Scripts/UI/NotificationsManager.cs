using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Classic.Core;
using UnityEngine.UI;

[DefaultExecutionOrder(150)]
public class NotificationsManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Gold gold;
    
    public GameObject achievementNotification;
    public TextMeshProUGUI achievementText;

    public GameObject storeNotification;
    public TextMeshProUGUI storeText;

    public bool highLightShop = false;
    public Color _defaultColor;

    public Button shopButton;
    

    private void OnEnable()
    {
       StartCoroutine(CalculateNotifications());
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

    private IEnumerator CalculateNotifications()
    {

        yield return new WaitForEndOfFrame();
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
            foreach (var storeItem in inventory.storeItems)
            {
                if(storeItem.currentTier >= storeItem.pricePerTier.Length) continue;
            
                if (storeItem.pricePerTier[storeItem.currentTier] <= gold.amount)
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

    private void OnValidate()
    {
        if (inventory == null)
        {
            inventory = FindObjectsByType<Inventory>(FindObjectsSortMode.None).First();
        }

            if (gold == null)
            {
                gold = FindObjectsByType<Gold>(FindObjectsSortMode.None)[0];
            }
        
    }
}
