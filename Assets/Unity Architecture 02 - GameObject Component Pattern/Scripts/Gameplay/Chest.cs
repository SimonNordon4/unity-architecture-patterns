using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class Chest : MonoBehaviour
    {
        public int minTier = 1;
        public int maxTier = 4;
        public Vector2Int options = new(0, 0);

        public AudioClip openSound;
        

        public List<ChestItem> items = new();
        
        public void GenerateItems()
        {
            Console.Log("\tChest.GenerateItems()", LogFilter.Chest, this);

            var numberOfItems = CalculateNumberOfItems();
            
            var chestManager = ChestManager.Instance;

            // Flags to check if a tier item has been added
            bool tier3Added = false;
            bool tier4Added = false;

            for (var i = 0; i < numberOfItems; i++)
            {
                var tier = CalculateItemTier();

                if (tier == 3) tier3Added = true;
                if (tier == 4) tier4Added = true;

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
        }


        private int CalculateNumberOfItems()
        {
            // we wanted a weight average between 2 - 5 items spawning.
            var itemsChance = Random.Range(0, 100);

            var numberOfItems = itemsChance switch
            {
                > 90 => 5,
                > 75 => 4,
                > 25 => 3,
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

            // We want better items to spawn as time goes on.
            var enemyProgress = (EnemyManager.Instance.enemyKillProgressCount / 400f) * 5f;
            
            // These pity numbers increase the chance of recieving a higher tier item each time we miss one.
            var itemTier = tierChance switch
            {
                var t when t > 99 - chestManager.tier4Pity - enemyProgress => 4,
                var t when t > 90 - chestManager.tier3Pity - enemyProgress => 3,
                var t when t > 75 - enemyProgress => 2,
                _ => 1,
            };

            // Increase the pity counters
            chestManager.tier4Pity++;
            chestManager.tier3Pity++;

            // Reset the pity counters if we recieve that item tier.
            if  (itemTier == 4)
                chestManager.tier4Pity = 0;

            else if (itemTier == 3)
                chestManager.tier3Pity=0;
            
            itemTier = Mathf.Clamp(itemTier, minTier, maxTier);

            return itemTier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.PlaySound(openSound);
                ChestManager.Instance.PickupChest(this);
                Destroy(gameObject);
            }
        }
    }
}