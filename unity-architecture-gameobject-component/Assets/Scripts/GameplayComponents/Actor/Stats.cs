using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameplayComponents.Actor
{
    [DefaultExecutionOrder(-10)]
    public class Stats : GameplayComponent
    {
        [SerializeField] private ActorStatsDefinition definition;
        [SerializeField] private List<Stat> stats = new();
        
        public void Initialize(ActorStatsDefinition newDefinition)
        {
            definition = newDefinition;
            stats.Clear();
            foreach (var stat in definition.stats)
            {
                stats.Add(new Stat(stat));
            }
        }

        private void Awake()
        {
            if (definition != null)
            {
                Initialize(definition);
            }
        }

        public Stat GetStat(StatType type)
        {
            return stats.FirstOrDefault(stat => stat.type == type);
        }

        public void SetStat(StatType type, float value)
        {
            var stat = GetStat(type);
            stat.value = value;
        }

        private void OnValidate()
        {
            // check if stats list is empty
            if (stats.Count == 0)
            {
                foreach (StatType type in Enum.GetValues(typeof(StatType)))
                {
                    stats.Add(new Stat(type));
                }
            }
            else
            {
                // if not, check if all stat types are present
                foreach (StatType type in Enum.GetValues(typeof(StatType)))
                {
                    if (!stats.Exists(stat => stat.type == type))
                    {
                        stats.Add(new Stat(type));
                    }
                }
            }
        }
    }
}