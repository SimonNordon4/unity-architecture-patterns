﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameplayComponents.Actor
{
    [DefaultExecutionOrder(-10)]
    public class Stats : GameplayComponent
    {
        [field:SerializeField] public List<Stat> stats { get; private set; }= new();
        
        public Action<Stat> onStatChanged;

        private void OnEnable()
        {
            foreach(var stat in stats)
            {
                stat.onModifierAdded += mod => OnModifierAdded(stat);
            }
        }

        private void OnModifierAdded(Stat stat)
        {
            onStatChanged?.Invoke(stat);
        }

        public Stat GetStat(StatType type)
        {
            return stats.FirstOrDefault(stat => stat.type == type);
        }

        public override void OnGameEnd()
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