using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        public ChestItems[] allChestItems;
        public float miniChestCooldown = 15f;
        public Chest chestPrefab;
        public Chest currentChest = null;

        [Header("Pity")]
        public int tier3Pity;
        public int tier4Pity;
        public int tier5Pity;

        [Header("UI")]
        public GameObject chestMenu;
        public GameObject hudMenu;



        public void Start()
        {
            // Chest has to spawn inside the level.
            if (chestBounds.magnitude > GameManager.instance.levelBounds.magnitude)
                chestBounds = GameManager.instance.levelBounds;

            allChestItems = new[] { tier1ChestItems, tier2ChestItems, tier3ChestItems, tier4ChestItems, tier5ChestItems };
            
            foreach (var chestItem in allChestItems)
                chestItem.SetTier();

            SpawnChest();
        }

        private void Update()
        {
            _timeSinceLastChestSpawn += Time.deltaTime;
            if (_timeSinceLastChestSpawn > chestSpawnTime)
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
            chest.GenerateItems(this);
        }

        private void SpawnBossChest(Vector3 position)
        {
            var chest = Instantiate(chestPrefab, position, Quaternion.identity);
            chest.minTier = 3;
            chest.maxTier = 5;
            chest.chestManager = this;
            chest.GenerateItems(this);
        }

        public void PickupChest(Chest chest)
        {
            // monitor stats.
            AccountManager.Instance.statistics.totalChestsOpened++;
            GameManager.instance.isGameActive = false;

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

            currentChest = chest;
            chestMenu.SetActive(true);
            hudMenu.SetActive(false);
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

            // Attempt to not dash when pressing space to select an item in teh chest menu.
            StartCoroutine(WaitOneFrameToUnpause());

            chestMenu.SetActive(false);
            hudMenu.SetActive(true);
            currentChest = null;
        }

        private IEnumerator WaitOneFrameToUnpause()
        {
            yield return new WaitForEndOfFrame();
            GameManager.instance.isGameActive = true;
        }

        public void Reset()
        {
            allChestItems = null;
            _timeSinceLastChestSpawn = 0f;

            tier3Pity = 0;
            tier4Pity = 0;
            tier5Pity = 0;
        }
    }
}
