using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class ChestManager : MonoBehaviour
    {
        [Header("References")]
        public PlayerManager playerManager;

        [Header("Chests")] 
        public float chestSpawnTime = 15f;
        private float _timeSinceLastChestSpawn = 0f;
        
        public Vector2 chestBounds = new(20f, 20f);
        public ChestItems tier1ChestItems;
        public ChestItems tier2ChestItems;
        public ChestItems tier3ChestItems;
        public ChestItems tier4ChestItems;
        public ChestItems tier5ChestItems;
        private ChestItem[][] _allItems;
        public float miniChestCooldown = 15f;
        public Chest chestPrefab;

        private int _pityLuck;
        public float pityLuckScaling = 1f;

        [Header("Chest UI")]
        public GameObject hudMenu;
        public GameObject chestMenu;
        public RectTransform chestItemButtonContainer;
        public UIChestItemButton chestItemButtonPrefab;
        private readonly List<UIChestItemButton> _chestItemButtons = new();

        public void Start()
        {
            _allItems = new[]
            {
            tier1ChestItems.chestItems.ToArray(),
            tier2ChestItems.chestItems.ToArray(),
            tier3ChestItems.chestItems.ToArray(),
            tier4ChestItems.chestItems.ToArray(),
            tier5ChestItems.chestItems.ToArray()
            };

            // Chest has to spawn inside the level.
            if (chestBounds.magnitude > GameManager.instance.levelBounds.magnitude)
                chestBounds = GameManager.instance.levelBounds;

            SpawnChest();
        }

        private void Update()
        {
            if(_timeSinceLastChestSpawn > chestSpawnTime)
            {
                _timeSinceLastChestSpawn = 0f;
                SpawnChest();
            }
        }

        public void ReduceChestSpawnTime()
        {
            _timeSinceLastChestSpawn += 1f;
        }

        private Vector3 GetRandomChestSpawn()
        {
            return new Vector3(Random.Range(-chestBounds.x, chestBounds.x), 0f, Random.Range(-chestBounds.y, chestBounds.y));
        }

        private void SpawnChest()
        {
            var chest = Instantiate(chestPrefab, GetRandomChestSpawn(), Quaternion.identity);
            chest.minTier = 1;
            chest.maxTier = 5;
            chest.chestManager = this;
        }

        private void SpawnBossChest(Vector3 position)
        {
            var chest = Instantiate(chestPrefab, position, Quaternion.identity);
            chest.minTier = 3;
            chest.maxTier = 5;
            chest.chestManager =  this;
        }

        public void PickupChest(Chest chest)
        {
            // monitor stats.
            AccountManager.Instance.statistics.totalChestsOpened++;
            GameManager.instance.isGameActive = false;
            
            chestMenu.SetActive(true);

            foreach (var cib in _chestItemButtons) Destroy(cib.gameObject);
            _chestItemButtons.Clear();

            // we wanted a weight average between 2 - 5 items spawning, with odds being increased by luck, which will be added later.
            var itemsChance = Random.Range(0, 100);
            var numberOfItems = 0;

            var luckFactor = 1 * 10f;
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

            // If the chest has a set number of items, then random range that number, evenly distributed.
            // Used for boss chests.
            if (chest.options.magnitude > 0) numberOfItems = Random.Range(chest.options.x, chest.options.y);

            // Store a hashset of all the items we have already added to the options, so we don't display duplicates.
            var alreadyAddedItems = new HashSet<ChestItem>();

            for (var i = 0; i < numberOfItems; i++)
            {
                var newChestItemButton = Instantiate(chestItemButtonPrefab, chestItemButtonContainer);
                _chestItemButtons.Add(newChestItemButton);

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
                    var item = possibleItems[x];
                    alreadyAddedItems.Add(item);
                    newChestItemButton.Initialize(item, this);
                    break;
                }

                List<Achievement> achievements = AccountManager.Instance.achievementSave.achievements
                    .Where(a => a.name == AchievementName.Open100Chests ||
                                a.name == AchievementName.Open1000Chests).ToList();
                foreach (var a in achievements)
                {
                    if (a.isCompleted) continue;
                    a.progress++;
                    if (a.progress >= a.goal)
                    {
                        a.isCompleted = true;
                        AccountManager.Instance.AchievementUnlocked(a);
                    }
                }

                // // For keyboard input.
                // _chestItemsWasdSelector.buttons.Clear();
                // foreach (var chestItemUI in _chestItemButtons)
                //     _chestItemsWasdSelector.buttons.Add(chestItemUI.GetComponent<Button>());
            }
        }

        // Return the tier
        private int GetRandomChestItemTier(Chest chest)
        {
            if (chest.minTier == chest.maxTier)
                return chest.minTier;

            var tier = 0;

            var chance = Random.Range(0, (200 - (1* 20f))) + 1* 20f;

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


            tier = Mathf.Clamp(tier, chest.minTier, chest.maxTier);
            Debug.Log(
                $"Item Tier:{tier} Pity: {_pityLuck} " +
                $"chance of T5: {(tier5Chance * 100):F1}% - " +
                $"T4: {(tier4Chance * 100):F1}% - " +
                $"T3: {(tier3Chance * 100):F1}%");

            return tier;
        }

        // private void UpdateItemUI()
        // {
        //     foreach (var item in _itemHoverImages) Destroy(item);
        //     _itemHoverImages.Clear();

        //     // Create a dictionary of every item and how many of them there are.
        //     var itemDictionary = new Dictionary<ChestItem, int>();
        //     foreach (var item in GameManager.instance.currentlyHeldItems)
        //         if (itemDictionary.ContainsKey(item))
        //             itemDictionary[item]++;
        //         else
        //             itemDictionary.Add(item, 1);

        //     // Create a new item hover image for each item in the dictionary.
        //     foreach (var item in itemDictionary.Keys)
        //     {
        //         var newHoverImage = Instantiate(itemHoverImagePrefab, itemHoverImageContainer);
        //         newHoverImage.Initialize(item, itemDictionary[item]);
        //         _itemHoverImages.Add(newHoverImage.gameObject);
        //     }
        // }

        public void ApplyItem(ChestItem item)
        {
            hudMenu.SetActive(true);
            GameManager.instance.currentlyHeldItems.Add(item);

            // Add modifiers to the stats.
            foreach (var mod in item.modifiers)
            {
                var stat = playerManager.Stats[mod.statType];
                stat.AddModifier(mod);

                // TODO: This might be broken.
                AccountManager.Instance.CheckIfHighestStat(mod.statType, stat.value);

                // If it's a max health mod, we need to also increase the current health.
                if (mod.statType == StatType.MaxHealth)
                {
                    AccountManager.Instance.statistics.totalDamageHealed += (int)mod.modifierValue;

                    var newHealth = Mathf.Clamp(playerManager.playerCurrentHealth + (int)mod.modifierValue, 1,
                        (int)playerManager.playerMaxHealth.value);

                    playerManager.playerCurrentHealth = newHealth;
                }
            }

            // UpdateStatsUI();
            // UpdateItemUI();

            // Attempt to not dash when pressing space to select an item in teh chest menu.
            StartCoroutine(WaitOneFrameToUnpause());
        }

        private IEnumerator WaitOneFrameToUnpause()
        {
            yield return new WaitForEndOfFrame();
            GameManager.instance.isGameActive = true;
        }

        public void Reset()
        {
            _pityLuck = 0;
        }
    }
}
