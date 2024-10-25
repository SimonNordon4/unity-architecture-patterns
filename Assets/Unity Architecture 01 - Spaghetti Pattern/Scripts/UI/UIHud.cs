using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIHud : MonoBehaviour
    {
        public PlayerManager playerManager;
        public TextMeshProUGUI roundTimer;
        public Image healthBar;
        public TextMeshProUGUI healthBarText;

        public void Update()
        {
            roundTimer.text = $"Round: {GameManager.instance.roundTime:00}";
            healthBar.fillAmount = playerManager.playerCurrentHealth / (float)playerManager.playerMaxHealth.value;
            healthBarText.text = $"{playerManager.playerCurrentHealth}/{playerManager.playerMaxHealth.value}";
        }
    }
}