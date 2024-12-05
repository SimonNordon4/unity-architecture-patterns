using System;
using TMPro;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIEnemyKillProgressionCounter : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI progressText;
        [SerializeField]private EnemyDirector enemyDirector;
        [SerializeField]private EnemySpawner enemySpawner;

        private void OnEnable()
        {
            enemySpawner.OnEnemyDied.AddListener(UpdateProgressText);
            enemySpawner.OnBossDied.AddListener(UpdateProgressText);
            UpdateProgressText(null);
        }

        private void OnDisable()
        {
            enemySpawner.OnEnemyDied.RemoveListener(UpdateProgressText);
            enemySpawner.OnBossDied.RemoveListener(UpdateProgressText);
        }

        private void UpdateProgressText(PoolableActor enemy)
        {
            if(enemyDirector.FightingBoss)
            {
                progressText.text = enemyDirector.BossesToDefeat > 1 ? "Defeat all Bosses" : "Defeat the boss";
                return;
            }

            progressText.text = $"Enemies Left: {enemyDirector.EnemyKillProgressCount} / {enemyDirector.EnemiesToKill}";
        }
    }
}
