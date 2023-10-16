using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    public class Stats : MonoBehaviour
    {
        public UnityEvent<StatType> onStatChanged { get; } = new();

        [field:SerializeField]
        public Stat playerHealth {get; private set;} = new(StatType.MaxHealth);
        [field:SerializeField]
        public Stat playerSpeed {get; private set;} = new(StatType.MoveSpeed);
        [field:SerializeField]
        public Stat block {get; private set;} = new(StatType.Block);
        [field:SerializeField]
        public Stat dodge {get; private set;} = new(StatType.Dodge);
        [field:SerializeField]
        public Stat revives {get; private set;} = new(StatType.Revives);
        [field:SerializeField]
        public Stat dashes {get; private set;} = new(StatType.Dashes);

        
        [field:SerializeField]
        public Stat projectileDamage {get;private set;} = new(StatType.RangedDamage);
        [field:SerializeField]
        public Stat range {get;private set;} = new(StatType.RangedRange);
        [field:SerializeField]
        public Stat fireRate {get;private set;} = new(StatType.RangedAttackSpeed);
        [field:SerializeField]
        public Stat projectileKnockBack {get;private set;} = new(StatType.RangedKnockBack);
        [field:SerializeField]
        public Stat projectilePierce {get;private set;} = new(StatType.RangedPierce);
        
        [field:SerializeField]
        public Stat meleeDamage {get;private set;} = new(StatType.MeleeDamage);
        [field:SerializeField]
        public Stat meleeRange {get;private set;} = new(StatType.MeleeRange);
        [field:SerializeField]
        public Stat attackSpeed {get;private set;} = new( StatType.MeleeAttackSpeed);
        [field:SerializeField]
        public Stat meleeKnockBack {get;private set;} = new(StatType.MeleeKnockBack);

        
        [field:SerializeField]
        public Stat luck {get;private set;} = new(StatType.Luck);
        [field:SerializeField]
        public Stat healthPackSpawnRate {get;private set;} = new(StatType.HealthPackDropRate);

        public Dictionary<StatType, Stat> statMap { get; } = new();

        private void Awake()
        {
            PopulateStats();
        }
        
        private void PopulateStats()
        {
            statMap.Add(StatType.MaxHealth, playerHealth);
            statMap.Add(StatType.MoveSpeed, playerSpeed);
            statMap.Add(StatType.Block, block);
            statMap.Add(StatType.Dodge, dodge);
            statMap.Add(StatType.Revives, revives);
            statMap.Add(StatType.Dashes, dashes);
            
            statMap.Add(StatType.RangedDamage, projectileDamage);
            statMap.Add(StatType.RangedRange, range);
            statMap.Add(StatType.RangedAttackSpeed, fireRate);
            statMap.Add(StatType.RangedKnockBack, projectileKnockBack);
            statMap.Add(StatType.RangedPierce, projectilePierce);
            
            statMap.Add(StatType.MeleeDamage, meleeDamage);
            statMap.Add(StatType.MeleeRange, meleeRange);
            statMap.Add(StatType.MeleeAttackSpeed, attackSpeed);
            statMap.Add(StatType.MeleeKnockBack, meleeKnockBack);
            
            statMap.Add(StatType.Luck, luck);
            statMap.Add(StatType.HealthPackDropRate, healthPackSpawnRate);
        }

        public void ApplyModifier(Modifier modifier)
        {
            statMap[modifier.statType].AddModifier(modifier);
            onStatChanged.Invoke(modifier.statType);
        }

        public void ApplyModifier(Modifier[] modifiers)
        {
            foreach (var modifier in modifiers)
            {
                ApplyModifier(modifier);
            }
        }
        
        public void ResetModifiers()
        {
            foreach (var stat in statMap.Values)
            {
                stat.Reset();
            }
        }
    }
}