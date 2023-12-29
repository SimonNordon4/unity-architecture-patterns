using System;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameObjectComponent.GameplayComponents;
using UnityEngine;

namespace GameplayComponents.Actor
{
    [DefaultExecutionOrder(-100)]
    public class Stats : GameplayComponent
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