using UnityEngine;
using TMPro;
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
            roundTimer.text = $"{(int)(GameManager.Instance.roundTime / 60):00}:{(int)(GameManager.Instance.roundTime % 60):00}";  
            healthBar.fillAmount = playerController.playerCurrentHealth / (float)playerController.playerMaxHealth.value;
            healthBarText.text = $"{playerController.playerCurrentHealth}/{playerController.playerMaxHealth.value}";

            if (enemyManager.activeBosses.Count > 0)
            {
                enemiesRemaining.text = "Defeat the Boss!";
            }
            else
            {
                enemiesRemaining.text = $"Enemies Killed: {enemyManager.enemyKillProgressCount}/400";    
            }
               
        }
    }
}