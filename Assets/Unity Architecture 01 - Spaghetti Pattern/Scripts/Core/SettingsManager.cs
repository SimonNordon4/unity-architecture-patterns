using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [DefaultExecutionOrder(-9)]
    public class SettingsManager : MonoBehaviour
    {
        private static SettingsManager _instance;
        public static SettingsManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<SettingsManager>();
                return _instance;
            }
            private set => _instance = value;
        }

        public GameObject hyperText;

        private void Awake()
        {
            Debug.Log("SettingsManager Start");
            musicVolume = AccountManager.Instance.settingsSave.musicVolume;
            sfxVolume = AccountManager.Instance.settingsSave.sfxVolume;
            showDamageNumbers = AccountManager.Instance.settingsSave.showDamageNumbers;
            showEnemyHealthBars = AccountManager.Instance.settingsSave.showEnemyHealthBars;
            isHyperMode = AccountManager.Instance.settingsSave.isHyperMode;
        }

        public bool showDamageNumbers = true;
        public bool showEnemyHealthBars = true;

        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        public bool isHyperMode = false;

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            AccountManager.Instance.settingsSave.musicVolume = volume;
            AccountManager.Instance.Save();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            AccountManager.Instance.settingsSave.sfxVolume = volume;
            AccountManager.Instance.Save();
        }

        public void SetShowEnemyHealthBars(bool show)
        {
            showEnemyHealthBars = show;

            // get every enemy in the scene
            var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.SetHealthBarVisibility(showEnemyHealthBars);
            }
            AccountManager.Instance.settingsSave.showEnemyHealthBars = show;
            AccountManager.Instance.Save();
        }

        public void ShowDamageNumbers(bool show)
        {
            showDamageNumbers = show;
            AccountManager.Instance.settingsSave.showDamageNumbers = show;
            AccountManager.Instance.Save();
        }
    }
}