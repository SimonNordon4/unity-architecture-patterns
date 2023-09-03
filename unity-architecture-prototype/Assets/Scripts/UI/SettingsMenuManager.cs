using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public Toggle healthBarToggle;
    public Toggle spawnRateToggle;

    private void OnEnable()
    {
        healthBarToggle.isOn = SettingsManager.instance.showEnemyHealthBars;
        spawnRateToggle.isOn = SettingsManager.instance.isNormalSpawnRate;
    }
}
