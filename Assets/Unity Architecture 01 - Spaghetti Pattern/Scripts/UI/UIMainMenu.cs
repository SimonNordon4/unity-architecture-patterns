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
        public Button Settings;
        public Button Quit;

        public GameObject UISettingsMenu;

        public void OnEnable()
        {
            Play.onClick.AddListener(OnPlayPressed);
            Settings.onClick.AddListener(OnSettingsPressed);
            Quit.onClick.AddListener(OnQuitPressed);
        }
        
        private void Start()
        {
            UISettingsMenu.SetActive(false);
        }

        public void OnDisable()
        {
            Play.onClick.RemoveListener(OnPlayPressed);
            Settings.onClick.RemoveListener(OnSettingsPressed);
            Quit.onClick.RemoveListener(OnQuitPressed);
        }

        private void OnPlayPressed()
        {
            // TODO: Game Manager Load Scene Main Game.
            UISettingsMenu.SetActive(false);
        }

        private void OnSettingsPressed()
        {
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