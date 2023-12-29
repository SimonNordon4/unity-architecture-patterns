using GameObjectComponent.Game;
using GameObjectComponent.GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Combat
{
    [RequireComponent(typeof(ActorStats))]
    [RequireComponent(typeof(CombatTarget))]
    public class MeleeAttack : GameplayComponent
    {
        [SerializeField] private BaseWeapon weapon;
        private ActorStats _stats;
        private CombatTarget _target;
        private Stat meleeDamage => _stats.Map[StatType.MeleeDamage];
        private Stat meleeKnockBack => _stats.Map[StatType.MeleeKnockBack];
        private Stat meleeRange => _stats.Map[StatType.MeleeRange];
        private Stat meleeAttackSpeed => _stats.Map[StatType.MeleeAttackSpeed];

        private float _timeSinceLastAttack = 0f;
        
        private void Start()
        {
            _stats = GetComponent<ActorStats>();
            _target = GetComponent<CombatTarget>();
        }

        private void Update()
        {
            var inverseAttackSpeed = 1f / meleeAttackSpeed.value;
            if(_timeSinceLastAttack < inverseAttackSpeed)
            {
                _timeSinceLastAttack += GameTime.deltaTime;
                return;
            }
            
            if(!_target.hasTarget) return;
            
            var distance = _target.targetDistance;
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