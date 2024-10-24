using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIMainMenu : MonoBehaviour
    {
        public Button Play;
        public Button Leaderboards;
        public Button Achievements;
        public Button Statistics;
        public Button Settings;
        public Button Quit;

        public GameObject UICharacterSelect;
        public GameObject UILeaderboardMenu;
        public GameObject UIAchievementMenu;
        public GameObject UIStatisticsMenu;
        public GameObject UISettingsMenu;

        public void OnEnable()
        {
            Play.onClick.AddListener(OnPlayPressed);
            Leaderboards.onClick.AddListener(OnLeaderboardsPressed);
            Achievements.onClick.AddListener(OnAchievementsPressed);
            Statistics.onClick.AddListener(OnStatisticsPressed);
            Settings.onClick.AddListener(OnSettingsPressed);
            Quit.onClick.AddListener(OnQuitPressed);
        }

        public void OnDisable()
        {
            Play.onClick.RemoveListener(OnPlayPressed);
            Leaderboards.onClick.RemoveListener(OnLeaderboardsPressed);
            Achievements.onClick.RemoveListener(OnAchievementsPressed);
            Statistics.onClick.RemoveListener(OnStatisticsPressed);
            Settings.onClick.RemoveListener(OnSettingsPressed);
            Quit.onClick.RemoveListener(OnQuitPressed);
        }

        private void OnPlayPressed()
        {
            UICharacterSelect.SetActive(true);
            UILeaderboardMenu.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UIStatisticsMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnLeaderboardsPressed()
        {
            UICharacterSelect.SetActive(false);
            UILeaderboardMenu.SetActive(true);
            UIAchievementMenu.SetActive(false);
            UIStatisticsMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnAchievementsPressed()
        {
            UICharacterSelect.SetActive(false);
            UILeaderboardMenu.SetActive(false);
            UIAchievementMenu.SetActive(true);
            UIStatisticsMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnStatisticsPressed()
        {
            UICharacterSelect.SetActive(false);
            UILeaderboardMenu.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UIStatisticsMenu.SetActive(true);
            UISettingsMenu.SetActive(false);
        }

        private void OnSettingsPressed()
        {
            UICharacterSelect.SetActive(false);
            UILeaderboardMenu.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UIStatisticsMenu.SetActive(false);
            UISettingsMenu.SetActive(true);
        }

        private void OnQuitPressed()
        {
            
        }
    }
}