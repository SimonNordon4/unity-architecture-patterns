using System.Collections.Generic;
using Classic.Core;
using UnityEngine;

namespace Classic.Items
{
    public class ChestSpawner : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField] private Vector2 edgeBuffer = new Vector2(2f, 2f);
        
        [SerializeField] private ChestItemsConfig tier1ChestItems;
        [SerializeField] private ChestItemsConfig tier2ChestItems;
        [SerializeField] private ChestItemsConfig tier3ChestItems;
        [SerializeField] private ChestItemsConfig tier4ChestItems;
        [SerializeField] private ChestItemsConfig tier5ChestItems;
        
        [SerializeField] private Chest miniChest;
        [SerializeField] private Chest mediumChest;
        [SerializeField] private Chest largeChest;

        private ChestItem[][] _allItems;
        private Dictionary<ChestType, Chest> _chestTypes;

        private void Start()
        {
            _allItems = new[]
            {
                tier1ChestItems.chestItems.ToArray(),
                tier2ChestItems.chestItems.ToArray(),
                tier3ChestItems.chestItems.ToArray(),
                tier4ChestItems.chestItems.ToArray(),
                tier5ChestItems.chestItems.ToArray()
            };
            
            _chestTypes = new Dictionary<ChestType, Chest>
            {
                {ChestType.Mini, miniChest},
                {ChestType.Medium, mediumChest},
                {ChestType.Large, largeChest}
            };
        }

        public Chest SpawnMiniChest()
        {
            var chest = Instantiate(miniChest, transform.position, Quaternion.identity);
            chest.numberOfItems = CalculateNumberOfItems(chest);
            chest.chestItems = CalculateChestItems(chest);
            
            var position = new Vector3(
                Random.Range(-level.bounds.x + edgeBuffer.x, level.bounds.x - edgeBuffer.x),
                0f,
                Random.Range(-level.bounds.y + edgeBuffer.y, level.bounds.y - edgeBuffer.y)
            );
            
            chest.transform.position = position;
            return chest;
        }

        public Chest SpawnChest(ChestType chestType)
        {
            var chest = Instantiate(_chestTypes[chestType], transform.position, Quaternion.identity);
            chest.numberOfItems = CalculateNumberOfItems(chest);
            chest.chestItems = CalculateChestItems(chest);
            
            var position = new Vector3(
                Random.Range(-level.bounds.x + edgeBuffer.x, level.bounds.x - edgeBuffer.x),
                0f,
                Random.Range(-level.bounds.y + edgeBuffer.y, level.bounds.y - edgeBuffer.y)
            );
            
            chest.transform.position = position;
            return chest;
        }

        public Chest SpawnChest(ChestType chestType, Vector3 position)
        {
            var chest = Instantiate(_chestTypes[chestType], transform.position, Quaternion.identity);
            chest.numberOfItems = CalculateNumberOfItems(chest);
            chest.chestItems = CalculateChestItems(chest);
            chest.transform.position = position;
            return chest;
        }


        private int CalculateNumberOfItems(Chest chest)
        {
            var itemsChance = Random.Range(0, 100);
            var numberOfItems = 0;

            var luckFactor = GameManager.instance.luck.value * 10f;
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

        private ChestItem[] CalculateChestItems(Chest chest)
        {
            // Store a hashset of all the items we have already added to the options, so we don't display duplicates.
            var alreadyAddedItems = new HashSet<ChestItem>();
            
            var items = new ChestItem[chest.numberOfItems];

            for (var i = 0; i < chest.numberOfItems; i++)
            {
                // Get the tier of the item to be spawned.
                var tier = GetRandomChestItemTier(chest);

                // Collect all items with a tier equal to or less than the chest tier
                var possibleItems = new List<ChestItem>();
                
                foreach (var chestItem in _allItems[tier - 1])
                {
                    // check if the chest item has already been added.
                    if (alreadyAddedItems.Contains(chestItem)) continue;
                    possibleItems.Add(chestItem);
                }

                // Now randomly select one of these possible items based on its probabilty
                var totalSpawnChance = 0;
                foreach (var chestItem in possibleItems) totalSpawnChance += chestItem.spawnChance;

                var randomSpawnChance = Random.Range(0, totalSpawnChance);
                var currentSpawnChance = 0;
                for (var j = 0; j < possibleItems.Count; j++)
                {
                    var x = j;
                    currentSpawnChance += possibleItems[x].spawnChance;

                    if (randomSpawnChance >= currentSpawnChance) continue;
                    // We have found the item to spawn
                    items[i] = possibleItems[x];
                    break;
                }
            }
            return items;
        }
        
        private int GetRandomChestItemTier(Chest chest)
        {
            if (chest.tiers.x == chest.tiers.y)
                return chest.tiers.x;

            var tier = 0;

            var chance = Random.Range(0, (200 - (GameManager.instance.luck.value * 20f))) +
                         GameManager.instance.luck.value * 20f;

            // 0 luck = 0 - 200.
            // 1 luck = 20 - 200.
            // 5 luck = 100 - 200.
            // 7 luck = 140 - 200.


            var tier5Chance = 0.005f + GameManager.instance.pityLuck * 0.0005f;
            var tier5Break = 200 - 200 * tier5Chance;

            var tier4Chance = 0.02f + GameManager.instance.pityLuck * 0.002f;
            var tier4Break = tier5Break - 200 * tier4Chance;

            var tier3Chance = 0.08f + GameManager.instance.pityLuck * 0.008f;
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
                GameManager.instance.pityLuck++;
            }
            else
            {
                GameManager.instance.pityLuck = 0;
            }
            tier = Mathf.Clamp(tier, chest.tiers.x, chest.tiers.y);
            return tier;
        }
    }
}