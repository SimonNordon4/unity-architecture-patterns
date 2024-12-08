using TMPro;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIEnemyKillProgressionCounter : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI progressText;
        [SerializeField]private EnemyDirector director;
        [SerializeField]private EnemyDiedEvent enemyDiedEvent;
        [SerializeField]private EnemyDiedEvent bossDiedEvent;

        private void OnEnable()
        {
            enemyDiedEvent.OnEnemyDied.AddListener(UpdateProgressText);
            bossDiedEvent.OnEnemyDied.AddListener(UpdateProgressText);
            UpdateProgressText(null);
        }

        private void OnDisable()
        {
            enemyDiedEvent.OnEnemyDied.RemoveListener(UpdateProgressText);
            bossDiedEvent.OnEnemyDied.RemoveListener(UpdateProgressText);
        }

        private void UpdateProgressText(GameObject enemy)
        {
            if(director.IsFightingBoss)
            {
                progressText.text = director.BossesLeft > 1 ? "Defeat all Bosses" : "Defeat the boss";
                return;
            }

            progressText.text = $"Enemies Left: {director.EnemyKillProgressCount} / {director.TotalEnemiesToKill}";
        }
    }
}
