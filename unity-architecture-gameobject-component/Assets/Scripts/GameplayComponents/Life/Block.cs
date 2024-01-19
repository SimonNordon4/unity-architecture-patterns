using GameplayComponents.Actor;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Life
{
    public class Block : GameplayComponent
    {
        [SerializeField] private Stats stats;
        private Stat _blockStat;
        public UnityEvent onFullBlock = new();
        
        private void Start()
        {
            _blockStat = stats.GetStat(StatType.Block);
        }

        public int CalculateBlock(int damageAmount)
        {
            var newDamage = Mathf.Max(0, damageAmount - (int)_blockStat.value);
            if(newDamage == 0) onFullBlock.Invoke();
            return newDamage;
        }
    }
}