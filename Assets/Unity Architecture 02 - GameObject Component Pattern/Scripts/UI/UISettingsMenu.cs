using TMPro;
using UnityArchitecture.GameObjectComponentPattern;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UISettingsMenu : MonoBehaviour
    {


        public Toggle healthBarToggle;
        public Toggle showDamageToggle;

        public Slider musicSlider;
        public Slider actionSlider;

        public TextMeshProUGUI musicText;
        public TextMeshProUGUI actionText;
        
        public Button backButton;

        private void OnEnable()
        {
            UserSettings.Instance.LoadSettings();
            healthBarToggle.isOn = UserSettings.Instance.ShowEnemyHealthBars;
            showDamageToggle.isOn = UserSettings.Instance.ShowDamageNumbers;

            musicSlider.value = UserSettings.Instance.MusicVolume;
            actionSlider.value = UserSettings.Instance.SfxVolume;
            musicText.text = $"{UserSettings.Instance.MusicVolume * 100f:F0}%";
            actionText.text = $"{UserSettings.Instance.SfxVolume * 100f:F0}%";
            
            if(backButton != null)
                backButton.onClick.AddListener(GameManager.Instance.HideSettingsMenu);
            healthBarToggle.onValueChanged.AddListener(UpdateHealthBars);
            showDamageToggle.onValueChanged.AddListener(UpdateDamageNumbers);
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            actionSlider.onValueChanged.AddListener(UpdateSoundVolume);
            
        }


        private void OnDisable()
        {
            if(backButton != null)
                backButton.onClick.RemoveAllListeners();
            healthBarToggle.onValueChanged.RemoveAllListeners();
            showDamageToggle.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
            actionSlider.onValueChanged.RemoveAllListeners();
        }

        public void UpdateMusicVolume(float volume)
        {
            var value = musicSlider.value;
            musicText.text = $"{value * 100:F0}%";
            UserSettings.Instance.SetMusicVolume(value);
        }

        public void UpdateSoundVolume(float volume)
        {
            var value = actionSlider.value;
            actionText.text = $"{value * 100:F0}%";
            UserSettings.Instance.SetSfxVolume(value);
        }

        public void UpdateHealthBars(bool health)
        {
            UserSettings.Instance.SetShowEnemyHealthBars(health);
        }

        public void UpdateDamageNumbers(bool damage)
        {
            UserSettings.Instance.SetShowDamageNumbers(damage);
        }
    }
}