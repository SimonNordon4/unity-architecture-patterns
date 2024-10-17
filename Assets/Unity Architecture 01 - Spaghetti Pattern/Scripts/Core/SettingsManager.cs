using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
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

        private void Start()
        {
            Debug.Log("SettingsManager Start");
            musicVolume = AccountManager.instance.settingsSave.musicVolume;
            sfxVolume = AccountManager.instance.settingsSave.sfxVolume;
            showDamageNumbers = AccountManager.instance.settingsSave.showDamageNumbers;
            showEnemyHealthBars = AccountManager.instance.settingsSave.showEnemyHealthBars;
            isHyperMode = AccountManager.instance.settingsSave.isHyperMode;
        }

        public bool showDamageNumbers = true;
        public bool showEnemyHealthBars = true;

        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        public bool isHyperMode = false;

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            AccountManager.instance.settingsSave.musicVolume = volume;
            AccountManager.instance.Save();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            AccountManager.instance.settingsSave.sfxVolume = volume;
            AccountManager.instance.Save();
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
            AccountManager.instance.settingsSave.showEnemyHealthBars = show;
            AccountManager.instance.Save();
        }

        public void ShowDamageNumbers(bool show)
        {
            showDamageNumbers = show;
            AccountManager.instance.settingsSave.showDamageNumbers = show;
            AccountManager.instance.Save();
        }
    }
}