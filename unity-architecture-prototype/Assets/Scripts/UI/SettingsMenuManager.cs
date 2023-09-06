using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public Toggle healthBarToggle;
    public Toggle showDamageToggle;

    private void OnEnable()
    {
        healthBarToggle.isOn = SettingsManager.instance.showEnemyHealthBars;
        showDamageToggle.isOn = SettingsManager.instance.showDamageNumbers;
    }
}
