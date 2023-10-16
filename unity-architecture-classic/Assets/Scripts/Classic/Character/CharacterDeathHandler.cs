using Classic.Actor;
using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterDeathHandler : ActorComponent
    {
        [SerializeField] private ActorHealth health;
        [SerializeField] private ActorStats stats;
        [SerializeField] private GameState gameState;
        private Stat _reviveStat;

        private void Start()
        {
            _reviveStat = stats.Map[StatType.Revives];
        }

        private void OnEnable()
        {
            health.OnDeath += OnDeath;
        }
        
        private void OnDisable()
        {
            health.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            if (_reviveStat.value > 0) return;
            gameState.GameOver();
        }
    }
}