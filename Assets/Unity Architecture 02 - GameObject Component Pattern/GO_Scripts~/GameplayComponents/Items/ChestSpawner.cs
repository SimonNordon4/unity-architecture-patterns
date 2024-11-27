using System.Collections.Generic;
using GameObjectComponent.App;
using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using GameplayComponents;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class ChestSpawner : GameplayComponent
    {
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private SoundManager soundManager;
        private Stat _luckStat;

        
        [SerializeField] private ItemTableDefinition tier1ChestItems;
        [SerializeField] private ItemTableDefinition tier2ChestItems;
        [SerializeField] private ItemTableDefinition tier3ChestItems;
        [SerializeField] private ItemTableDefinition tier4ChestItems;
        [SerializeField] private ItemTableDefinition tier5ChestItems;
        
        [SerializeField] private ChestDefinition miniChest;
        [SerializeField] private ChestDefinition mediumChest;
        [SerializeField] private ChestDefinition largeChest;

        private ChestItemDefinition[][] _allItems;
        private Dictionary<ChestType, Chest> _chestTypes;

        private int _pityLuck;

        private void Start()
        {
            _allItems = new[]
            {
                tier1ChestItems.items.ToArray(),
                tier2ChestItems.items.ToArray(),
                tier3ChestItems.items.ToArray(),
                tier4ChestItems.items.ToArray(),
                tier5ChestItems.items.ToArray()
            };
            
            _chestTypes = new Dictionary<ChestType, Chest>
            {
                {ChestType.Mini, miniChest.chestPrefab},
                {ChestType.Medium, mediumChest.chestPrefab},
                {ChestType.Large, largeChest.chestPrefab}
            };
            
            _luckStat = stats.GetStat(StatType.Luck);
        }

        public Chest SpawnChest(ChestType chestType, Vector3 position)
        {
            var chest = Instantiate(_chestTypes[chestType], transform.position, Quaternion.identity);
            if(chest.TryGetComponent<GameplayStateController>(out var gameState))
                gameState.Construct(state);
            if(chest.TryGetComponent<SoundProxy>(out var soundProxy))
                soundProxy.Construct(soundManager);
            chest.numberOfItems = CalculateNumberOfItems(chest);
            chest.chestItems = CalculateChestItems(chest);
            chest.transform.position = position;
            chest.gameObject.SetActive(true);
            return chest;
        }


        private int CalculateNumberOfItems(Chest chest)
        {
            var itemsChance = Random.Range(0, 100);
            var numberOfItems = 0;

            var luckFactor = _luckStat.value * 10f;
            itemsChance += (int)luckFactor;

            switch (itemsChance)
            {
                case > 99:
                    numberOfItems = 5;
                    break;
                case > 90:
                    numberOfItems = 4;
                    break;
                case > 70:
                    numberOfItems = 3;
                    break;
                default:
                    numberOfItems = 2;
                    break;
            }
            
            numberOfItems = Mathf.Clamp(numberOfItems, chest.options.x, chest.options.y);
            
            return numberOfItems;
        }

        private ChestItemDefinition[] CalculateChestItems(Chest chest)
        {
            // Store a hashset of all the items we have already added to the options, so we don't display duplicates.
            var alreadyAddedItems = new HashSet<ChestItemDefinition>();
            var items = new ChestItemDefinition[chest.numberOfItems];

            for (var i = 0; i < chest.numberOfItems; i++)
            {
                // Get the tier of the item to be spawned.
                var tier = GetRandomChestItemTier(chest);

                // Get a random item from the tier.
                var item = _allItems[tier - 1][Random.Range(0, _allItems[tier - 1].Length)];
                
                // If we have already added this item, try again.
                var x = 0;
                while (alreadyAddedItems.Contains(item) || x < 20)
                {
                    item = _allItems[tier - 1][Random.Range(0, _allItems[tier - 1].Length)];
                    x++;
                }
                
                // Add the item to the list of items we have already added.
                alreadyAddedItems.Add(item);
                items[i] = item;
            }
            return items;
        }
        
        private int GetRandomChestItemTier(Chest chest)
        {
            if (chest.tiers.x == chest.tiers.y)
                return chest.tiers.x;

            var tier = 0;

            var luck = _luckStat.value;
            var chance = Random.Range(0, (200 - (luck * 20f))) +
                         luck * 20f;

            // 0 luck = 0 - 200.
            // 1 luck = 20 - 200.
            // 5 luck = 100 - 200.
            // 7 luck = 140 - 200.


            var tier5Chance = 0.005f + _pityLuck * 0.0005f;
            var tier5Break = 200 - 200 * tier5Chance;

            var tier4Chance = 0.02f + _pityLuck * 0.002f;
            var tier4Break = tier5Break - 200 * tier4Chance;

            var tier3Chance = 0.08f + _pityLuck * 0.008f;
            var tier3Break = tier4Break - 200 * tier3Chance;

            // T2 remains at 16% and does not scale.
            var tier2Break = tier3Break - 200 * 0.16f;

            if (chance >= tier5Break)
                tier = 5;
            else if (chance >= tier4Break)
                tier = 4;
            else if (chance >= tier3Break)
                tier = 3;
            else if (chance >= tier2Break)
                tier = 2;
            else
                tier = 1;

            if (tier < 3)
            {
                _pityLuck++;
            }
            else
            {
                _pityLuck = 0;
            }
            tier = Mathf.Clamp(tier, chest.tiers.x, chest.tiers.y);
            return tier;
        }
    }
}