﻿using System;
using GameObjectComponent.Game;
using GameplayComponents;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class AddStoreItemsToStatsHandler : GameplayComponent
    {
        [SerializeField] private Store store;
        [SerializeField] private Stats stats;
        
        public override void OnGameStart()
        {
            // go through every store item
            foreach (var storeItem in store.purchasedStoreItems)
            {
                if(storeItem.currentUpgrade == 0) continue;
                var modifier = storeItem.upgrades[storeItem.currentUpgrade - 1].modifier;
                var stat = stats.GetStat(modifier.statType);
                stat.AddModifier(modifier);
            }
        }
    }
}