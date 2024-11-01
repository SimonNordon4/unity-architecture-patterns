using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class Chest : MonoBehaviour
    {
        public int minTier = 1;
        public int maxTier = 5;
        public Vector2Int options = new(0, 0);

        public SoundDefinition openSound;
        

        public List<ChestItem> items = new();
        
        public void GenerateItems()
        {
            Console.Log("\tChest.GenerateItems()", LogFilter.Chest, this);

            var numberOfItems = CalculateNumberOfItems();
            
            var chestManager = ChestManager.Instance;

            // Flags to check if a tier item has been added
            bool tier3Added = false;
            bool tier4Added = false;
            bool tier5Added = false;

            for (var i = 0; i < numberOfItems; i++)
            {
                var tier = CalculateItemTier();

                if (tier == 3) tier3Added = true;
                if (tier == 4) tier4Added = true;
                if (tier == 5) tier5Added = true;

                var currentChestTier = chestManager.allChestItems[tier - 1];
                var validPotentialItems = new List<ChestItem>();

                foreach (var chestItem in currentChestTier.chestItems)
                {
                    if (items.Contains(chestItem)) continue;
                    validPotentialItems.Add(chestItem);
                }

                var randomItemIndex = Random.Range(0, validPotentialItems.Count);
                items.Add(validPotentialItems[randomItemIndex]);
            }

            chestManager.tier3Pity = tier3Added ? 0 : chestManager.tier3Pity + 1;
            chestManager.tier4Pity = tier4Added ? 0 : chestManager.tier4Pity + 1;
            chestManager.tier5Pity = tier5Added ? 0 : chestManager.tier5Pity + 1;
        }


        private int CalculateNumberOfItems()
        {
            // we wanted a weight average between 2 - 5 items spawning.
            var itemsChance = Random.Range(0, 100);

            var numberOfItems = itemsChance switch
            {
                > 95 => 5,
                > 85 => 4,
                > 50 => 3,
                _ => 2,
            };
            
            Console.Log($"\t\t Item Chance {itemsChance} resulting in {numberOfItems} items", LogFilter.Chest, this);
            return  numberOfItems;
        }

        // Return the tier
        private int CalculateItemTier()
        {
            var chestManager = ChestManager.Instance;
            
            var tierChance = Random.Range(0, 100);
            
            // These pity numbers increase the chance of recieving a higher tier item each time we miss one.
            var itemTier = tierChance switch
            {
                var t when t > 99 - chestManager.tier5Pity => 5,
                var t when t > 95 - chestManager.tier4Pity => 4,
                var t when t > 85 - chestManager.tier3Pity => 3,
                var t when t > 75 => 2,
                _ => 1,
            };

            // Increase the pity counters
            chestManager.tier5Pity++;
            chestManager.tier4Pity++;
            chestManager.tier3Pity++;

            // Reset the pity counters if we recieve that item tier.
            if (itemTier == 5)
                chestManager.tier5Pity = 0;

            else if  (itemTier == 4)
                chestManager.tier4Pity = 0;

            else if (itemTier == 3)
                chestManager.tier3Pity=0;

            // This will set it so that the max tier per block is 2,3,4,5... etc to stop stupid scaling.
            var adjustedMaxTier = Mathf.Clamp(EnemyManager.Instance.currentBlockIndex + 3, 1, 5);
            // clamp the results to the min max tiers
            itemTier = Mathf.Clamp(itemTier, minTier, adjustedMaxTier);
            
            Console.Log($"\t\t Tier Chance {tierChance} resulting in tier {itemTier} with pity chances " +
                        $"{chestManager.tier3Pity}, {chestManager.tier4Pity}, {chestManager.tier5Pity}",
                LogFilter.Chest, this);

            return itemTier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.instance.PlaySound(openSound);
                ChestManager.Instance.PickupChest(this);
                Destroy(gameObject);
            }
        }
    }
}