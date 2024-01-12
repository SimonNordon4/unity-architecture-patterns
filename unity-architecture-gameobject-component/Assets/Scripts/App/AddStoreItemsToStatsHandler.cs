using System;
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
                if(storeItem.upgradesPurchased == 0) continue;
                
                // go through every upgrade purchased
                for (var i = 0; i < storeItem.upgradesPurchased; i++)
                {
                    var modifier = storeItem.storeItemDefinition.upgrades[storeItem.upgradesPurchased - 1].modifier;
                    var stat = stats.GetStat(modifier.statType);
                    stat.AddModifier(modifier);
                }
            }
        }
    }
}