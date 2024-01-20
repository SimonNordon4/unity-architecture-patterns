using System.Collections;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField]private StatisticsManager statisticsManager;
        [SerializeField]private GameState gameState;
        [SerializeField]private PopUpScheduler popUpScheduler;

        private void OnEnable()
        {
            gameState.OnGameStart += OnGameStarted;
            gameState.OnGameLost += OnGameLost;
        }

        private void OnDisable()
        {
            gameState.OnGameStart -= OnGameStarted;
            gameState.OnGameLost -= OnGameLost;
        }
        
        private void OnGameStarted()
        {
            var gamePlayed = statisticsManager.GetStatistic(StatisticType.GamesPlayed);
            if (gamePlayed.highestValue > 1) return;
            StartCoroutine(GameTutorial());
        }

        private IEnumerator GameTutorial()
        {
            yield return new WaitForSeconds(1f);
            popUpScheduler.SchedulePopup("Used WASD Move");
            yield return new WaitForSeconds(4f);
            popUpScheduler.SchedulePopup("F to Pause and View your Stats");
            yield return new WaitForSeconds(4f);
            popUpScheduler.SchedulePopup("Collect Chests to get Upgrades");
            yield return null;
        }

        private IEnumerator DeathTutorial()
        {
            yield return new WaitForSeconds(1f);
            popUpScheduler.SchedulePopup("Claim Achievements for Gold");
            yield return new WaitForSeconds(4f);
            popUpScheduler.SchedulePopup("Buy Items in the Store for permanent Upgrades");
            yield return null;
        }
        
        private void OnGameLost()
        {
            StopAllCoroutines();
            // get times died
            var timesDied = statisticsManager.GetStatistic(StatisticType.Deaths);
            
            // check if it's equal less than or equal to 1
            if (timesDied.highestValue <= 1)
            {
                // if so, show tutorial
                StartCoroutine(DeathTutorial());
            }
        }
    }
}