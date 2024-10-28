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
        public ChestManager chestManager;

        public List<ChestItem> items = new();

        public void GenerateItems(ChestManager chestManager)
        {
            // Store a hashset of all the items we have already added to the options, so we don't add duplicates.
            var alreadyAddedItems = new HashSet<ChestItem>();

            // Figure out how many items this chest will have.
            var numberOfItems = CalculateNumberOfItems();

            for (var i = 0; i < numberOfItems; i++)
            {
                // Get the tier of the item to be spawned.
                var tier = CalculateItemTier();

                // Get the list of items based on the select tier.
                var currentChestTier = chestManager.allChestItems[tier - 1];

                // We want to make sure we don't add the same item twice.
                var validPotentialItems = new List<ChestItem>();
                foreach (var chestItem in currentChestTier.chestItems)
                {
                    // check if the chest item has already been added.
                    if (alreadyAddedItems.Contains(chestItem)) continue;
                    validPotentialItems.Add(chestItem);
                }

                // Now just select a random item from the list of valid items.
                var randomItemIndex = Random.Range(0, validPotentialItems.Count);
                items.Add(validPotentialItems[randomItemIndex]);
            }
        }


        private int CalculateNumberOfItems()
        {
            // we wanted a weight average between 2 - 5 items spawning.
            var itemsChance = Random.Range(0, 100);

            var numberOfItems = itemsChance switch
            {
                > 90 => 5,
                > 75 => 4,
                > 50 => 3,
                _ => 2,
            };

            return  numberOfItems;
        }

        // Return the tier
        private int CalculateItemTier()
        {
            var tierChance = Random.Range(0, 100);

            // These pity numbers increase the chance of recieving a higher tier item each time we miss one.
            var itemTier = tierChance switch
            {
                var t when t > 99 - chestManager.tier5Pity => 5,
                var t when t > 90 - chestManager.tier4Pity => 4,
                var t when t > 50 - chestManager.tier3Pity => 3,
                _ => 2,
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

            // clamp the results to the min max tiers
            itemTier = Mathf.Clamp(itemTier, minTier, maxTier);

            return itemTier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.instance.PlaySound(openSound);
                chestManager.PickupChest(this);
                Destroy(gameObject);
            }
        }
    }
}