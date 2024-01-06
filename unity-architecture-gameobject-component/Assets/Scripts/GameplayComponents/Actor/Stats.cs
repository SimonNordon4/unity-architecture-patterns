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
        [SerializeField] private List<Stat> stats = new();
        
        public Stat GetStat(StatType type)
        {
            return stats.FirstOrDefault(stat => stat.type == type);
        }

        public override void OnGameStart()
        {
            ResetStats();
        }
          
        private void ResetStats()
        {
            foreach(var stat in stats)
            {
                stat.Reset();
            }
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