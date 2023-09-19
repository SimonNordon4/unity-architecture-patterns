using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public Toggle healthBarToggle;
    public Toggle showDamageToggle;
    
    public Slider musicSlider;
    public Slider actionSlider;
    
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI actionText;

    private void OnEnable()
    {
        healthBarToggle.isOn = SettingsManager.instance.showEnemyHealthBars;
        showDamageToggle.isOn = SettingsManager.instance.showDamageNumbers;
        
        musicSlider.value = SettingsManager.instance.musicVolume;
        actionSlider.value = SettingsManager.instance.sfxVolume;
        musicText.text = $"{SettingsManager.instance.musicVolume * 100f:F0}%";
        actionText.text = $"{SettingsManager.instance.sfxVolume * 100f:F0}%";
    }

    public void UpdateMusicVolume()
    {
        var value = musicSlider.value;
        musicText.text = $"{(value * 100):F0}%";
        SettingsManager.instance.SetMusicVolume(value);
    }

    public void UpdateSoundVolume()
    {
        var value = actionSlider.value;
        actionText.text = $"{value * 100:F0}%";
        SettingsManager.instance.SetSfxVolume(value);
    }
}
