using GameObjectComponent.App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UISettingsMenu : MonoBehaviour
    {
        [SerializeField]private Settings settings;
        
        [SerializeField]private Toggle healthBarToggle;
        [SerializeField]private Toggle showDamageToggle;
        [SerializeField]private Slider musicSlider;
        [SerializeField]private Slider actionSlider;
        [SerializeField]private TextMeshProUGUI musicText;
        [SerializeField]private TextMeshProUGUI actionText;
        

        private void OnEnable()
        {
            healthBarToggle.isOn = settings.showEnemyHealthBars;
            showDamageToggle.isOn = settings.showDamageNumbers;
            musicSlider.value = settings.musicVolume;
            actionSlider.value = settings.sfxVolume;
            musicText.text = $"{settings.musicVolume * 100f:F0}%";
            actionText.text = $"{settings.sfxVolume * 100f:F0}%";
        }

        public void UpdateMusicVolume(float value)
        {
            musicText.text = $"{(value * 100):F0}%";
            settings.SetMusicVolume(value);
        }

        public void UpdateSoundVolume(float value)
        {
            actionText.text = $"{value * 100:F0}%";
            settings.SetSfxVolume(value);
        }
        
        public void UpdateShowEnemyHealthBars(bool value)
        {
            settings.SetShowEnemyHealthBars(value);
        }
        
        public void UpdateShowDamageNumbers(bool value)
        {
            settings.SetShowDamageNumbers(value);
        }
    }

}