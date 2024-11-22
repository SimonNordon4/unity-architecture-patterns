using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIHud : MonoBehaviour
    {
        public EnemyManager enemyManager;
        public PlayerController playerController;
        public TextMeshProUGUI roundTimer;
        public TextMeshProUGUI enemiesRemaining;
        public Image healthBar;
        public TextMeshProUGUI healthBarText;

        public void Update()
        {
            roundTimer.text = $"Round: {GameManager.Instance.roundTime:00}";
            healthBar.fillAmount = playerController.playerCurrentHealth / (float)playerController.playerMaxHealth.value;
            healthBarText.text = $"{playerController.playerCurrentHealth}/{playerController.playerMaxHealth.value}";
            enemiesRemaining.text = $"Enemies Killed: {enemyManager.totalEnemiesKilled}";   
        }
    }
}