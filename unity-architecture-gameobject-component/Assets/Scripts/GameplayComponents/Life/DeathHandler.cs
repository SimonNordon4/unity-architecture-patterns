using System;
using GameplayComponents.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Life
{
    public class DeathHandler : GameplayComponent
    {
        [SerializeField] private Stats stats;
        [SerializeField] private Health health;
        private Stat _reviveStat;

        public UnityEvent onRevived = new();
        public UnityEvent onDeath = new();
        public Action<DeathHandler> OnDeath;

        private void Start()
        { 
            _reviveStat = stats.GetStat(StatType.Revives);
        }

        private void OnEnable()
        {
            health.OnHealthDepleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            health.OnHealthDepleted -= OnHealthDepleted;
        }
        
        private void OnHealthDepleted()
        {
            if (_reviveStat.value > 0)
            {
                _reviveStat.value--;
                onRevived.Invoke();
                health.SetHealth(health.maxHealth);
                return;
            }
            
            onDeath?.Invoke();
            OnDeath?.Invoke(this);
        }

        public override void OnGameStart()
        {
            _reviveStat = stats.GetStat(StatType.Revives);
        }
        
    }
}