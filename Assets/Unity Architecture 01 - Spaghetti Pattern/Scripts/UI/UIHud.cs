using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIHud : MonoBehaviour
    {
        public EnemyManager enemyManager;
        [FormerlySerializedAs("playerManager")] public PlayerController playerController;
        public TextMeshProUGUI roundTimer;
        public TextMeshProUGUI enemiesRemaining;
        public Image healthBar;
        public TextMeshProUGUI healthBarText;

        public void Update()
        {
            roundTimer.text = $"Round: {GameManager.Instance.roundTime:00}";
            healthBar.fillAmount = playerController.playerCurrentHealth / (float)playerController.playerMaxHealth.value;
            healthBarText.text = $"{playerController.playerCurrentHealth}/{playerController.playerMaxHealth.value}";
            var currentBlock = enemyManager.enemyBlocks[enemyManager.currentBlockIndex];
            enemiesRemaining.text = $"Enemies Left: {currentBlock.enemiesKilled}/{currentBlock.enemiesToKill}";   

            if (currentBlock.enemiesKilled >= currentBlock.enemiesToKill)
            {
                enemiesRemaining.text = "Defeat the Boss!";
            }
        }
    }
}