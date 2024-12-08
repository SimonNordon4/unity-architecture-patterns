using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class Chest : MonoBehaviour
    {
        public UnityEvent<Chest> OnChestOpened = new();
        [field:SerializeField] public int MinTier {get;private set;} = 1;
        [field:SerializeField] public int MaxTier {get;private set;} = 4;
        
        [SerializeField]private EnemyDirector director;

        private int _tier3Pity;
        private int _tier4Pity;
        public List<ChestItem> items = new();

        public void Construct(int minTier, int maxTier, int tier3Pity, int tier4Pity)
        {
            MinTier = minTier;
            MaxTier = maxTier;
            _tier3Pity = tier3Pity;
            _tier4Pity = tier4Pity;
        }
        
        public (int,int) GenerateItems(ChestItems[] chestItems)
        {
            var numberOfItems = CalculateNumberOfItems();
            
            // Flags to check if a tier item has been added
            bool tier3Added = false;
            bool tier4Added = false;

            for (var i = 0; i < numberOfItems; i++)
            {
                var tier = CalculateItemTier();

                if (tier == 3) tier3Added = true;
                if (tier == 4) tier4Added = true;

                var currentChestTier = chestItems[tier - 1];
                var validPotentialItems = new List<ChestItem>();

                foreach (var chestItem in currentChestTier.chestItems)
                {
                    if (items.Contains(chestItem)) continue;
                    validPotentialItems.Add(chestItem);
                }

                var randomItemIndex = Random.Range(0, validPotentialItems.Count);
                items.Add(validPotentialItems[randomItemIndex]);
            }

            _tier3Pity = tier3Added ? 0 : _tier3Pity + 1;
            _tier4Pity = tier4Added ? 0 : _tier4Pity + 1;
            
            return (_tier3Pity, _tier4Pity);
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
            
            return  numberOfItems;
        }

        // Return the tier
        private int CalculateItemTier()
        {
            var tierChance = Random.Range(0, 100);

            // We want better items to spawn as time goes on.
            var enemyProgress = director.ProgressPercentage * 5f;
            
            // These pity numbers increase the chance of recieving a higher tier item each time we miss one.
            var itemTier = tierChance switch
            {
                var t when t > 99 - _tier4Pity - enemyProgress => 4,
                var t when t > 90 - _tier3Pity - enemyProgress => 3,
                var t when t > 75 - enemyProgress => 2,
                _ => 1,
            };

            // Increase the pity counters
            _tier4Pity++;
            _tier3Pity++;

            // Reset the pity counters if we recieve that item tier.
            if  (itemTier == 4)
                _tier4Pity = 0;

            else if (itemTier == 3)
                _tier3Pity=0;
            
            itemTier = Mathf.Clamp(itemTier, MinTier, MaxTier);

            return itemTier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnChestOpened.Invoke(this);
                Destroy(gameObject);
            }

        }
    }
}