using GameplayComponents.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Life
{
    public class Dodge : GameplayComponent
    {
        [SerializeField] private Stats stats;
        private Stat _dodgeStat;
        public UnityEvent onDodged = new();
        
        private void Start()
        {
            _dodgeStat = stats.GetStat(StatType.Dodge);
        }

        public bool CalculateDodge()
        {
            var chance = _dodgeStat.value;
            var random = Random.Range(0, 100);
            
            if (random > chance) return false;
            onDodged.Invoke();
            return true;
        }
    }
}