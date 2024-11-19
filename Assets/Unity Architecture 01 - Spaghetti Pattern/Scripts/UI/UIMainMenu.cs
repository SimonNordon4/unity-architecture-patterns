using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    /// <summary>
    /// Controls the main menu UI. Responsible for switching between the other UI Scenes.
    /// </summary>
    public class UIMainMenu : MonoBehaviour
    {
        public Button Play;
        public Button Achievements;
        public Button Settings;
        public Button Quit;

        public GameObject UICharacterSelect;
        public GameObject UIAchievementMenu;
        public GameObject UISettingsMenu;

        public void OnEnable()
        {
            Play.onClick.AddListener(OnPlayPressed);
            Settings.onClick.AddListener(OnSettingsPressed);
            Quit.onClick.AddListener(OnQuitPressed);
        }
        
        private void Start()
        {
            // All other screens should be off at scene load
            UICharacterSelect.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        public void OnDisable()
        {
            Play.onClick.RemoveListener(OnPlayPressed);
            Achievements.onClick.RemoveListener(OnAchievementsPressed);
            Settings.onClick.RemoveListener(OnSettingsPressed);
            Quit.onClick.RemoveListener(OnQuitPressed);
        }

        private void OnPlayPressed()
        {
            UICharacterSelect.SetActive(true);
            UIAchievementMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnLeaderboardsPressed()
        {
            UICharacterSelect.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnAchievementsPressed()
        {
            UICharacterSelect.SetActive(false);
            UIAchievementMenu.SetActive(true);
            UISettingsMenu.SetActive(false);
        }

        private void OnStatisticsPressed()
        {
            UICharacterSelect.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UISettingsMenu.SetActive(false);
        }

        private void OnSettingsPressed()
        {
            UICharacterSelect.SetActive(false);
            UIAchievementMenu.SetActive(false);
            UISettingsMenu.SetActive(true);
        }

        private void OnQuitPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
    }
}