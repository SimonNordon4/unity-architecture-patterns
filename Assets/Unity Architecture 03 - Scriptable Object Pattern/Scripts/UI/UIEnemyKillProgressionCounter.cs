using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIEnemyKillProgressionCounter : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI progressText;
        [SerializeField]private EnemyDirector director;
        [FormerlySerializedAs("enemyDiedEvent")] [SerializeField]private ActorDiedEvent actorDiedEvent;
        [SerializeField]private ActorDiedEvent bossDiedEvent;

        private void OnEnable()
        {
            actorDiedEvent.OnActorDied.AddListener(UpdateProgressText);
            bossDiedEvent.OnActorDied.AddListener(UpdateProgressText);
            UpdateProgressText(null);
        }

        private void OnDisable()
        {
            actorDiedEvent.OnActorDied.RemoveListener(UpdateProgressText);
            bossDiedEvent.OnActorDied.RemoveListener(UpdateProgressText);
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
