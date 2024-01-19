using GameObjectComponent.Game;
using GameplayComponents.Actor;
using GameplayComponents.Combat.Weapons;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class RangedAttack : GameplayComponent
    {
        [SerializeField] private BaseWeapon weapon;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        private Stat rangedDamage => stats.GetStat(StatType.RangedDamage);
        private Stat rangedKnockBack => stats.GetStat(StatType.RangedKnockBack);
        private Stat rangedRange => stats.GetStat(StatType.RangedRange);
        private Stat rangeAttackSpeed => stats.GetStat(StatType.RangedAttackSpeed);
        private Stat rangedPierce => stats.GetStat(StatType.RangedPierce);

        private float _timeSinceLastAttack = 0f;
        
        private void Update()
        {
            var inverseAttackSpeed = 1f / rangeAttackSpeed.value;
            if(_timeSinceLastAttack < inverseAttackSpeed)
            {
                _timeSinceLastAttack += GameTime.deltaTime;
                return;
            }

            if (!target.hasTarget)
            {
                return;
            }
            
            if (target.targetDistance > rangedRange.value)
            {
                return;
            }
            
            var info = new WeaponStatsInfo
            {
                Damage = (int)rangedDamage.value,
                KnockBack = rangedKnockBack.value,
                Range = rangedRange.value,
                AttackSpeed = rangeAttackSpeed.value,
                Pierce = (int)rangedPierce.value,
            };
            
            weapon.Attack(info, target);
            
            _timeSinceLastAttack = 0f;
        }
    }
}