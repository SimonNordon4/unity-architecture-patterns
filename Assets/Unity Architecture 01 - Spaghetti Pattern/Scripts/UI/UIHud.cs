using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIHud : MonoBehaviour
    {
        public TextMeshProUGUI roundTimer;
        public Image healthBar;
        public TextMeshProUGUI healthBarText;

        public void Update()
        {
            roundTimer.text = $"Round: {GameManager.instance.roundTime:00}";
            healthBar.fillAmount = GameManager.instance.playerCurrentHealth / (float)GameManager.instance.playerMaxHealth.value;
            healthBarText.text = $"{GameManager.instance.playerCurrentHealth}/{GameManager.instance.playerMaxHealth.value}";
        }
    }
}