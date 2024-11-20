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

        private void OnEnable()
        {
            healthBarToggle.isOn = GameManager.Instance.showEnemyHealthBars;
            showDamageToggle.isOn = GameManager.Instance.showDamageNumbers;

            musicSlider.value = GameManager.Instance.musicVolume;
            actionSlider.value = GameManager.Instance.sfxVolume;
            musicText.text = $"{GameManager.Instance.musicVolume * 100f:F0}%";
            actionText.text = $"{GameManager.Instance.sfxVolume * 100f:F0}%";
        }

        public void UpdateMusicVolume()
        {
            var value = musicSlider.value;
            musicText.text = $"{(value * 100):F0}%";
            GameManager.Instance.SetMusicVolume(value);
        }

        public void UpdateSoundVolume()
        {
            var value = actionSlider.value;
            actionText.text = $"{value * 100:F0}%";
            GameManager.Instance.SetSfxVolume(value);
        }
    }
}