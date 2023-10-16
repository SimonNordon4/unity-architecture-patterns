using System;
using System.Collections.Generic;
using UnityEngine;

namespace Classic.Actors
{
    public class ActorStats : ActorComponent
    {
        [SerializeField] private ActorStatsDefinition definition;
        public readonly Dictionary<StatType, Stat> Map = new();
        
        public void Initialize(ActorStatsDefinition newDefinition)
        {
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                Map[type] = newDefinition.GetStatByType(type);
            }
        }

        private void Awake()
        {
            if (definition != null)
            {
                Initialize(definition);
            }
        }
    }
}