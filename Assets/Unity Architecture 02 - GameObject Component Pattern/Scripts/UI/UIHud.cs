using UnityEngine;
using TMPro;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIHud : MonoBehaviour
    {
        public EnemyManager enemyManager;
        public PlayerController playerController;
        public TextMeshProUGUI roundTimer;
        public TextMeshProUGUI enemiesRemaining;


        public void Update()
        {
            roundTimer.text = $"{(int)(GameManager.Instance.roundTime / 60):00}:{(int)(GameManager.Instance.roundTime % 60):00}";  


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