using TMPro;
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
            GameManager.Instance.LoadSettings();
            healthBarToggle.isOn = GameManager.Instance.showEnemyHealthBars;
            showDamageToggle.isOn = GameManager.Instance.showDamageNumbers;

            musicSlider.value = GameManager.Instance.musicVolume;
            actionSlider.value = GameManager.Instance.sfxVolume;
            musicText.text = $"{GameManager.Instance.musicVolume * 100f:F0}%";
            actionText.text = $"{GameManager.Instance.sfxVolume * 100f:F0}%";
            
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
            musicText.text = $"{(value * 100):F0}%";
            GameManager.Instance.SetMusicVolume(value);
            GameManager.Instance.SaveSettings();
        }

        public void UpdateSoundVolume(float volume)
        {
            var value = actionSlider.value;
            actionText.text = $"{value * 100:F0}%";
            GameManager.Instance.SetSfxVolume(value);
            GameManager.Instance.SaveSettings();
        }

        public void UpdateHealthBars(bool health)
        {
            GameManager.Instance.SetShowEnemyHealthBars(health);
            GameManager.Instance.SaveSettings();
        }

        public void UpdateDamageNumbers(bool damage)
        {
            GameManager.Instance.ShowDamageNumbers(damage);
            GameManager.Instance.SaveSettings();
        }
    }
}