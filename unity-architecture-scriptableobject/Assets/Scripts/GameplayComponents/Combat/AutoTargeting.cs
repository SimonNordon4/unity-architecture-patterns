using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class AutoTargeting : GameplayComponent
    {
        [SerializeField] private CombatTarget target;
        [SerializeField] private Stats stats;
        
        [SerializeField]private float updateInterval = 0.2f;
        private float _timeSinceLastUpdate = 0f;
        
        private void Update()
        {
            if(_timeSinceLastUpdate < updateInterval)
            {
                _timeSinceLastUpdate += Time.deltaTime;
                return;
            }
            
            _timeSinceLastUpdate = 0f;
            
            var range = stats.GetStat(StatType.RangedRange).value;
            var meleeRange = stats.GetStat(StatType.MeleeRange).value;
            var maxRange = Mathf.Max(range, meleeRange);

            target.GetClosestTarget(maxRange);
        }
        
    }
}