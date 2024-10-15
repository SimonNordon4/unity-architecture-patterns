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

    public Color _defaultColor;


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
    }
}
