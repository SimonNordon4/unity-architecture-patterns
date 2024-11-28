using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [DefaultExecutionOrder(-10)]
    public class Stats : MonoBehaviour
    {
        [field:SerializeField] public Stat MaxHealth { get; private set; } = new(5, StatType.MaxHealth);
        [field:SerializeField] public Stat HealthRegen { get; private set; } = new(0, StatType.HealthRegen);
        [field:SerializeField] public Stat Speed { get; private set; } = new(5, StatType.Speed);
        [field:SerializeField] public Stat Armor { get; private set; } = new(0, StatType.Armor);
        [field:SerializeField] public Stat Dodge { get; private set; } = new(0, StatType.Dodge);
        [field:SerializeField] public Stat Damage { get; private set; } = new(1, StatType.Damage);
        [field:SerializeField] public Stat CritChance { get; private set; } = new(0, StatType.CritChance);
        [field:SerializeField] public Stat Range { get; private set; } = new(5, StatType.Range);
        [field:SerializeField] public Stat Firerate { get; private set; } = new(5, StatType.FireRate);
        [field:SerializeField] public Stat KnockBack { get; private set; } = new(1, StatType.KnockBack);
        [field:SerializeField] public Stat Pierce { get; private set; } = new(1, StatType.Pierce);

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
