using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] private SettingsManager settings;
    
    public Toggle healthBarToggle;
    public Toggle showDamageToggle;
    
    public Slider musicSlider;
    public Slider actionSlider;
    
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI actionText;

    private void OnEnable()
    {
        healthBarToggle.isOn = settings.showEnemyHealthBars;
        showDamageToggle.isOn = settings.showDamageNumbers;
        
        musicSlider.value = settings.musicVolume;
        actionSlider.value = settings.sfxVolume;
        musicText.text = $"{settings.musicVolume * 100f:F0}%";
        actionText.text = $"{settings.sfxVolume * 100f:F0}%";
    }

    public void UpdateMusicVolume()
    {
        var value = musicSlider.value;
        musicText.text = $"{(value * 100):F0}%";
        settings.SetMusicVolume(value);
    }

    public void UpdateSoundVolume()
    {
        var value = actionSlider.value;
        actionText.text = $"{value * 100:F0}%";
        settings.SetSfxVolume(value);
    }
}
