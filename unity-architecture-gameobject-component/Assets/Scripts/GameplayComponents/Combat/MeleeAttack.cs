using GameObjectComponent.Game;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents.Combat
{

    public class MeleeAttack : GameplayComponent
    {
        [SerializeField] private BaseWeapon weapon;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        private Stat meleeDamage => stats.GetStat(StatType.MeleeDamage);
        private Stat meleeKnockBack => stats.GetStat(StatType.MeleeKnockBack);
        private Stat meleeRange => stats.GetStat(StatType.MeleeRange);
        private Stat meleeAttackSpeed => stats.GetStat(StatType.MeleeAttackSpeed);

        private float _timeSinceLastAttack = 0f;
        
        private void Update()
        {
            var inverseAttackSpeed = 1f / meleeAttackSpeed.value;
            if(_timeSinceLastAttack < inverseAttackSpeed)
            {
                _timeSinceLastAttack += GameTime.deltaTime;
                return;
            }
            
            if(!target.hasTarget) return;
            
            var distance = target.targetDistance;
            if (distance > meleeRange.value) return;
            
            var info = new MeleeStatsInfo
            {
                Damage = (int)meleeDamage.value,
                KnockBack = meleeKnockBack.value,
                Range = meleeRange.value,
                AttackSpeed = meleeAttackSpeed.value
            };
            
            weapon.Attack(info);
            
            _timeSinceLastAttack = 0f;
        }
    }
    
    public struct MeleeStatsInfo
    {
        public int Damage;
        public float KnockBack;
        public float Range;
        public float AttackSpeed;
    }
}