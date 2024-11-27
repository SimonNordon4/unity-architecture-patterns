using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class Stats : MonoBehaviour
    {
        public Stat MaxHealth { get; private set; } = new(5, StatType.MaxHealth);
        public Stat HealthRegen { get; private set; } = new(0, StatType.HealthRegen);
        public Stat Speed { get; private set; } = new(5, StatType.Speed);
        public Stat Armor { get; private set; } = new(0, StatType.Armor);
        public Stat Dodge { get; private set; } = new(0, StatType.Dodge);
        public Stat Damage { get; private set; } = new(1, StatType.Damage);
        public Stat CritChance { get; private set; } = new(0, StatType.CritChance);
        public Stat Range { get; private set; } = new(5, StatType.Range);
        public Stat Firerate { get; private set; } = new(5, StatType.FireRate);
        public Stat KnockBack { get; private set; } = new(1, StatType.KnockBack);
        public Stat Pierce { get; private set; } = new(1, StatType.Pierce);

        private List<Stat> stats = new();

        private void Awake()
        {
            stats.Add(MaxHealth);
            stats.Add(HealthRegen);
            stats.Add(Speed);
            stats.Add(Armor);
            stats.Add(Dodge);
            stats.Add(Damage);
            stats.Add(CritChance);
            stats.Add(Range);
            stats.Add(Firerate);
            stats.Add(KnockBack);
            stats.Add(Pierce);
        }

        public Stat GetStat(StatType statType)
        {
            return stats.FirstOrDefault(stat => stat.StatType == statType);
        }
    }
}
