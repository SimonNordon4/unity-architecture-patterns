using GameObjectComponent.GameplayComponents.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectComponent.GameplayComponents
{
    public class Dodge : GameplayComponent
    {
        [SerializeField] private ActorStats stats;
        private Stat _dodgeStat;
        public UnityEvent onDodged = new();
        
        private void Start()
        {
            _dodgeStat = stats.Map[StatType.Dodge];
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