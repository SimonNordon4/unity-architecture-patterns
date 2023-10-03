using System.Collections.Generic;
using UnityEngine;

namespace Classic.Core
{
    public class Stats : MonoBehaviour
    {
        [field:SerializeField]
        public Stat playerHealth {get; private set;} = new(10,StatType.PlayerHealth);
        [field:SerializeField]
        public Stat playerSpeed {get; private set;} = new(5,StatType.PlayerSpeed);
        [field:SerializeField]
        public Stat block {get; private set;} = new(0,StatType.Block);
        [field:SerializeField]
        public Stat dodge {get; private set;} = new(0,StatType.Dodge);
        [field:SerializeField]
        public Stat revives {get; private set;} = new(0, StatType.Revives);
        [field:SerializeField]
        public Stat dashes {get; private set;} = new(0, StatType.Dashes);

        
        [field:SerializeField]
        public Stat projectileDamage {get;private set;} = new(1, StatType.RangeDamage);
        [field:SerializeField]
        public Stat range {get;private set;} = new(5, StatType.Range);
        [field:SerializeField]
        public Stat fireRate {get;private set;} = new(0.5f, StatType.FireRate);
        [field:SerializeField]
        public Stat projectileKnockBack {get;private set;} = new(1, StatType.ProjectileKnockBack);
        [field:SerializeField]
        public Stat projectilePierce {get;private set;} = new(0, StatType.ProjectilePierce);
        
        [field:SerializeField]
        public Stat meleeDamage {get;private set;} = new(3, StatType.MeleeDamage);
        [field:SerializeField]
        public Stat meleeRange {get;private set;} = new(1, StatType.MeleeRange);
        [field:SerializeField]
        public Stat attackSpeed {get;private set;} = new(3, StatType.AttackSpeed);
        [field:SerializeField]
        public Stat meleeKnockBack {get;private set;} = new(3, StatType.MeleeKnockBack);
        [field:SerializeField]
        public Stat arc {get;private set;} = new(45, StatType.Arc);
        
        [field:SerializeField]
        public Stat luck {get;private set;} = new(0, StatType.Luck);
        [field:SerializeField]
        public Stat healthPackSpawnRate {get;private set;} = new(5, StatType.HealthPackSpawnRate);

        public Dictionary<StatType, Stat> statMap { get; } = new();

        private void Awake()
        {
            PopulateStats();
        }
        
        private void PopulateStats()
        {
            statMap.Add(StatType.PlayerHealth, playerHealth);
            statMap.Add(StatType.PlayerSpeed, playerSpeed);
            statMap.Add(StatType.Block, block);
            statMap.Add(StatType.Dodge, dodge);
            statMap.Add(StatType.Revives, revives);
            statMap.Add(StatType.Dashes, dashes);
            
            statMap.Add(StatType.RangeDamage, projectileDamage);
            statMap.Add(StatType.Range, range);
            statMap.Add(StatType.FireRate, fireRate);
            statMap.Add(StatType.ProjectileKnockBack, projectileKnockBack);
            statMap.Add(StatType.ProjectilePierce, projectilePierce);
            
            statMap.Add(StatType.MeleeDamage, meleeDamage);
            statMap.Add(StatType.MeleeRange, meleeRange);
            statMap.Add(StatType.AttackSpeed, attackSpeed);
            statMap.Add(StatType.MeleeKnockBack, meleeKnockBack);
            statMap.Add(StatType.Arc, arc);
            
            statMap.Add(StatType.Luck, luck);
            statMap.Add(StatType.HealthPackSpawnRate, healthPackSpawnRate);
        }

        public void ApplyModifier(Modifier modifier)
        {
            statMap[modifier.statType].AddModifier(modifier);
        }

        public void ApplyModifier(Modifier[] modifiers)
        {
            foreach (var modifier in modifiers)
            {
                statMap[modifier.statType].AddModifier(modifier);
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