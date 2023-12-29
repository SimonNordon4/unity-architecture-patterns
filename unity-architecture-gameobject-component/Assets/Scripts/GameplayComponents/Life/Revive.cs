using GameplayComponents.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Life
{
    public class Revive : GameplayComponent
    {
        [SerializeField] private Stats stats;
        [SerializeField] private Health health;
        private Stat _reviveStat;

        public UnityEvent onRevived = new();
        
        private void Start() => _reviveStat = stats.Map[StatType.Revives];
        private void OnEnable() => health.OnDeath += OnDeath;
        private void OnDisable() => health.OnDeath -= OnDeath;
        
        private void OnDeath()
        {
            if (_reviveStat.value <= 0) return;
            _reviveStat.value--;
            health.Reset();
            onRevived.Invoke();
        }
        
    }
}