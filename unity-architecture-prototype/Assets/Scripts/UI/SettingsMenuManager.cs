using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public Toggle healthBarToggle;
    public Toggle spawnRateToggle;
    public Toggle showDamageToggle;

    private void OnEnable()
    {
        healthBarToggle.isOn = SettingsManager.instance.showEnemyHealthBars;
        spawnRateToggle.isOn = SettingsManager.instance.isNormalSpawnRate;
        showDamageToggle.isOn = SettingsManager.instance.showDamageNumbers;
    }
}
