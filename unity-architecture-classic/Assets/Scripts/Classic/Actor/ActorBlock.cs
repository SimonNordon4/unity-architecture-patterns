using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actor
{
    public class ActorBlock : ActorComponent
    {
        [SerializeField] private ActorStats stats;
        private Stat _blockStat;
        public UnityEvent onFullBlock = new();
        
        private void Start()
        {
            _blockStat = stats.Map[StatType.Block];
        }

        public int CalculateBlock(int damageAmount)
        {
            var newDamage = Mathf.Max(0, damageAmount - (int)_blockStat.value);
            if(newDamage == 0) onFullBlock.Invoke();
            return newDamage;
        }
    }
}