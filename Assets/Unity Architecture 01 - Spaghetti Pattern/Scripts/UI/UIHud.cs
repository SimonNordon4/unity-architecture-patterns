using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIHud : MonoBehaviour
    {
        public EnemyManager enemyManager;
        public PlayerManager playerManager;
        public TextMeshProUGUI roundTimer;
        public TextMeshProUGUI enemiesRemaining;
        public Image healthBar;
        public TextMeshProUGUI healthBarText;

        public void Update()
        {
            roundTimer.text = $"Round: {GameManager.Instance.roundTime:00}";
            healthBar.fillAmount = playerManager.playerCurrentHealth / (float)playerManager.playerMaxHealth.value;
            healthBarText.text = $"{playerManager.playerCurrentHealth}/{playerManager.playerMaxHealth.value}";
            var currentBlock = enemyManager.enemyBlocks[enemyManager.currentBlockIndex];
            enemiesRemaining.text = $"Enemies Left: {currentBlock.enemiesKilled}/{currentBlock.enemiesToKill}";   

            if (currentBlock.enemiesKilled >= currentBlock.enemiesToKill)
            {
                enemiesRemaining.text = "Defeat the Boss!";
            }
        }
    }
}