using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UISettingsMenu : MonoBehaviour
    {
        [SerializeField]private UserSettings userSettings;

        public Toggle healthBarToggle;
        public Toggle showDamageToggle;

        public Slider musicSlider;
        public Slider actionSlider;

        public TextMeshProUGUI musicText;
        public TextMeshProUGUI actionText;
        
        private void OnEnable()
        {
            userSettings.LoadSettings();
            healthBarToggle.isOn = userSettings.ShowEnemyHealthBars;
            showDamageToggle.isOn = userSettings.ShowDamageNumbers;

            musicSlider.value = userSettings.MusicVolume;
            actionSlider.value = userSettings.SfxVolume;
            musicText.text = $"{userSettings.MusicVolume * 100f:F0}%";
            actionText.text = $"{userSettings.SfxVolume * 100f:F0}%";
            
            healthBarToggle.onValueChanged.AddListener(UpdateHealthBars);
            showDamageToggle.onValueChanged.AddListener(UpdateDamageNumbers);
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            actionSlider.onValueChanged.AddListener(UpdateSoundVolume);
        }


        private void OnDisable()
        {
            healthBarToggle.onValueChanged.RemoveAllListeners();
            showDamageToggle.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
            actionSlider.onValueChanged.RemoveAllListeners();
        }

        public void UpdateMusicVolume(float volume)
        {
            var value = musicSlider.value;
            musicText.text = $"{value * 100:F0}%";
            userSettings.SetMusicVolume(value);
        }

        public void UpdateSoundVolume(float volume)
        {
            var value = actionSlider.value;
            actionText.text = $"{value * 100:F0}%";
            userSettings.SetSfxVolume(value);
        }

        public void UpdateHealthBars(bool health)
        {
            userSettings.SetShowEnemyHealthBars(health);
        }

        public void UpdateDamageNumbers(bool damage)
        {
            userSettings.SetShowDamageNumbers(damage);
        }
    }
}