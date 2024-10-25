using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace UnityArchitecture.SpaghettiPattern
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<GameManager>();
                return _instance;
            }
            private set => _instance = value;
        }

        private PlayerManager playerManager;

        #region Variables

        [Header("Round")] public float roundDuration = 20f;
        public float roundTime;
        public bool isPaused = false;
        public bool isGameActive = false;
        public Vector2 levelBounds = new(25f, 25f);

        public readonly List<ChestItem> currentlyHeldItems = new();

        [Header("UI")]
        public GameObject pauseMenu;
        public GameObject hudMenu;
        public GameObject gameOverMenu;
        public GameObject winMenu;

        // public List<TextMeshProUGUI> GoldTexts = new();
        // public List<TextMeshProUGUI> GoldSubTexts = new();

        // [Header("Stat UI")] public List<RectTransform> StatContainers = new();
        // private Dictionary<StatType, List<TextMeshProUGUI>> statTexts = new();
        // public Color defaultStatColor;
        // public Color plusStatColor;
        // public Color minusStatColor;
        // public TMP_FontAsset statFont;

        // [Header("Item UI")] public RectTransform itemHoverImageContainer;
        // private readonly List<GameObject> _itemHoverImages = new();
        // public UIChestItemHoverImage itemHoverImagePrefab;


        // Chest Item
        // public GameObject chestItemMenu;
        // public RectTransform chestItemButtonContainer;
        // public UIChestItemButton chestItemButtonPrefab;
        // private readonly List<UIChestItemButton> _chestItemButtons = new();

        [Header("Chests")] public Vector2 chestBounds = new(20f, 20f);
        public ChestItemsConfig tier1ChestItems;
        public ChestItemsConfig tier2ChestItems;
        public ChestItemsConfig tier3ChestItems;
        public ChestItemsConfig tier4ChestItems;
        public ChestItemsConfig tier5ChestItems;
        private ChestItem[][] _allItems;
        public float miniChestCooldown = 15f;
        private float _timeSinceLastMiniChest = 0.0f;
        private bool _nextMiniChest = false;
        public Chest miniChestPrefab;
        public Chest mediumChestPrefab;
        public Chest largeChestPrefab;

        private int _pityLuck;
        public float pityLuckScaling = 1f;

        // public UIWasdButtonSelector _chestItemsWasdSelector;

        [Header("Health Packs")] 
        public GameObject HealthPackPrefab;

        #endregion

        #region Unity Functions

        private void Start()
        {
            playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None)[0];
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;

            _allItems = new[]
            {
            tier1ChestItems.chestItems.ToArray(),
            tier2ChestItems.chestItems.ToArray(),
            tier3ChestItems.chestItems.ToArray(),
            tier4ChestItems.chestItems.ToArray(),
            tier5ChestItems.chestItems.ToArray()
            };

            // Chest has to spawn inside the level.
            if (chestBounds.magnitude > levelBounds.magnitude)
                chestBounds = levelBounds;
            isGameActive = true;
            StartNewGame();
        }

        private void Update()
        {
            if (isGameActive)
            {
                roundTime += Time.deltaTime;

                if (_nextMiniChest)
                {
                    _timeSinceLastMiniChest += Time.deltaTime;
                    if (_timeSinceLastMiniChest > miniChestCooldown)
                    {
                        SpawnMiniChest();
                        _timeSinceLastMiniChest = 0.0f;
                        _nextMiniChest = false;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F)) TogglePauseGame();
        }

        #endregion

        #region Application State

        public void StartNewGame()
        {
            Debug.Log("Start New Game");
            HideAll();
            AccountManager.instance.statistics.gamesPlayed++;
            hudMenu.SetActive(true);
            isGameActive = true;
            roundTime = 0f;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            SpawnMiniChest();
            // clear all items
            currentlyHeldItems.Clear();
            //UpdateItemUI();
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Wasd, 2f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Chest, 12f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Dash, 22f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Pause, 28f);
            AudioManager.instance.OnStartGame();
        }

        public void TogglePauseGame()
        {
            // If the game is active and we're not already paused.
            if (isGameActive && !isPaused)
            {
                HideAll();
                isGameActive = false;
                isPaused = true;
                pauseMenu.SetActive(true);
            }
            // If the game is not active and we are paused.
            else if (!isGameActive && isPaused)
            {
                HideAll();
                isGameActive = true;
                isPaused = false;
                hudMenu.SetActive(true);
            }
            // Do nothing otherwise.
        }

        public void ResetGame()
        {
            _pityLuck = 0;
            isPaused = false;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            Debug.Log("Reset Game");
            var playerController = FindFirstObjectByType<PlayerManager>();
            playerController.ResetPlayer();

            var enemyManager = FindFirstObjectByType<EnemyManagerOld>();
            enemyManager.ResetEnemyManager();

            var projectiles = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
            foreach (var projectile in projectiles) Destroy(projectile.gameObject);

            // find all objects with tag "Spawn Indicator" and destroy them.
            var spawnIndicators = GameObject.FindGameObjectsWithTag("Spawn Indicator");
            foreach (var spawnIndicator in spawnIndicators) Destroy(spawnIndicator);

            playerManager.ResetStats();

            // Remove all chests.
            var chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
            foreach (var chest in chests) Destroy(chest.gameObject);

            // Remove all healthpacks
            var healthPacks = FindObjectsByType<HealthPackController>(FindObjectsSortMode.None);
            foreach (var healthPack in healthPacks) Destroy(healthPack.gameObject);
        }

        public void WinGame()
        {
            AccountManager.instance.Save();
            HideAll();
            winMenu.SetActive(true);
            isGameActive = false;
            AccountManager.instance.statistics.gamesWon++;

            // check if faster time
            if (AccountManager.instance.statistics.fastestWin == 0 ||
                roundTime < AccountManager.instance.statistics.fastestWin)
                AccountManager.instance.statistics.fastestWin = roundTime;

            List<Achievement> achievements = AccountManager.instance.achievementSave.achievements
                .Where(a => a.name == AchievementName.BeatTheGame ||
                            a.name == AchievementName.BeatTheGame10Times).ToList();
            foreach (var a in achievements)
            {
                if (a.isCompleted) return;
                a.progress++;
                if (a.progress >= a.goal)
                {
                    a.isCompleted = true;
                    AccountManager.instance.AchievementUnlocked(a);
                }
            }

            var under1hour = AccountManager.instance.achievementSave.achievements
                .First(x => x.name == AchievementName.WinInUnder1Hour);

            if (!under1hour.isCompleted)
            {
                if (under1hour.progress > roundTime)
                    under1hour.progress = (int)roundTime;
                under1hour.isCompleted = roundTime < under1hour.goal;
            }


            var under45mins = AccountManager.instance.achievementSave.achievements
                .First(x => x.name == AchievementName.WinInUnder45Minutes);

            if (!under45mins.isCompleted)
            {
                if (under45mins.progress > roundTime)
                    under45mins.progress = (int)roundTime;
                under45mins.isCompleted = roundTime < under45mins.goal;
            }


            var under30mins = AccountManager.instance.achievementSave.achievements
                .First(x => x.name == AchievementName.WinInUnder30Minutes);

            if (!under30mins.isCompleted)
            {
                if (under30mins.progress > roundTime)
                    under30mins.progress = (int)roundTime;
                under30mins.isCompleted = roundTime < under30mins.goal;
            }

            roundTime = 0f;
            AudioManager.instance.StopMusic();
        }

        public void LoseGame()
        {
            AudioManager.instance.StopMusic();
            Debug.Log("Lose Game");
            HideAll();
            gameOverMenu.SetActive(true);
            isGameActive = false;

            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Buy, 2f);
        }

        private void HideAll()
        {
            hudMenu.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);
            // chestItemMenu.SetActive(false);
        }

        public void QuitApplication()
        {
            AccountManager.instance.Save();

            Debug.Log("Quit Application");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }

        #endregion

        // private void PopulateStatsUI()
        // {
        //     foreach (var statContainer in StatContainers)
        //     {
        //         // get all the children of the stat container and destroy them.
        //         foreach (Transform child in statContainer) Destroy(child.gameObject);

        //         foreach (var key in _stats.Keys)
        //         {
        //             Debug.Log("Creating stat for: " + key);
        //             var newStatText = Instantiate(new GameObject(key.ToString()).AddComponent<TextMeshProUGUI>(),
        //                 statContainer);
        //             newStatText.fontSize = 24;
        //             newStatText.font = statFont;
        //             newStatText.color = defaultStatColor;
        //             // set the width of the text transform to 400
        //             newStatText.rectTransform.sizeDelta = new Vector2(400, 32);

        //             if (statTexts.ContainsKey(key))
        //             {
        //                 statTexts[key].Add(newStatText);
        //             }
        //             else
        //             {
        //                 statTexts.Add(key, new List<TextMeshProUGUI>() { newStatText });
        //             }
        //         }
        //     }

        //     // clean up bonky stats created in the hierarchy.
        //     var bonkyStats = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        //     foreach (var bonkyText in bonkyStats)
        //     {
        //         if (bonkyText.transform.parent == null)
        //             Destroy(bonkyText.gameObject);
        //     }


        //     UpdateStatsUI();
        // }

        // private void UpdateStatsUI()
        // {
        //     foreach (var key in statTexts.Keys)
        //     {
        //         var statTypeText = key.ToString();
        //         for (var i = 1; i < statTypeText.Length; i++)
        //             if (char.IsUpper(statTypeText[i]))
        //             {
        //                 statTypeText = statTypeText.Insert(i, " ");
        //                 i++;
        //             }

        //         foreach (var newStatText in statTexts[key])
        //         {
        //             newStatText.text = $"{statTypeText}: {_stats[key].value:F1}";
        //             // check if the stat value is above, below or equal to the initial value.
        //             if (_stats[key].value > _stats[key].initialValue)
        //                 newStatText.color = plusStatColor;
        //             else if (_stats[key].value < _stats[key].initialValue)
        //                 newStatText.color = minusStatColor;
        //             else
        //                 newStatText.color = defaultStatColor;
        //         }
        //     }
        // }

        #region Chest Creation

        private Vector3 GetRandomChestPosition()
        {
            return new Vector3(Random.Range(-chestBounds.x, chestBounds.x), 0.5f, Random.Range(-chestBounds.y, chestBounds.y));
        }

        private void SpawnMiniChest()
        {
            var miniChest = Instantiate(miniChestPrefab, GetRandomChestPosition(), Quaternion.identity);
            miniChest.minTier = 1;
            miniChest.maxTier = 5;
        }

        public void PickupChest(Chest chest)
        {
            // monitor stats.
            AccountManager.instance.statistics.totalChestsOpened++;


            // If we pickup a mini chest, we can start spawning the next mini chest.
            if (chest.chestType == ChestType.Mini) _nextMiniChest = true;
            isGameActive = false;
            HideAll();
            // chestItemMenu.SetActive(true);

            // foreach (var cib in _chestItemButtons) Destroy(cib.gameObject);
            // _chestItemButtons.Clear();

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
                // var newChestItemButton = Instantiate(chestItemButtonPrefab, chestItemButtonContainer);
                // _chestItemButtons.Add(newChestItemButton);

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
                    // newChestItemButton.Initialize(item);
                    break;
                }

                List<Achievement> achievements = AccountManager.instance.achievementSave.achievements
                    .Where(a => a.name == AchievementName.Open100Chests ||
                                a.name == AchievementName.Open1000Chests).ToList();
                foreach (var a in achievements)
                {
                    if (a.isCompleted) continue;
                    a.progress++;
                    if (a.progress >= a.goal)
                    {
                        a.isCompleted = true;
                        AccountManager.instance.AchievementUnlocked(a);
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

        public void ApplyItem(ChestItem item)
        {
            HideAll();
            hudMenu.SetActive(true);
            currentlyHeldItems.Add(item);

            // Add modifiers to the stats.
            foreach (var mod in item.modifiers)
            {
                var stat = playerManager.Stats[mod.statType];
                stat.AddModifier(mod);

                // TODO: This might be broken.
                AccountManager.instance.CheckIfHighestStat(mod.statType, stat.value);

                // If it's a max health mod, we need to also increase the current health.
                if (mod.statType == StatType.MaxHealth)
                {
                    AccountManager.instance.statistics.totalDamageHealed += (int)mod.modifierValue;

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
            isGameActive = true;
        }

        // private void UpdateItemUI()
        // {
        //     foreach (var item in _itemHoverImages) Destroy(item);
        //     _itemHoverImages.Clear();

        //     // Create a dictionary of every item and how many of them there are.
        //     var itemDictionary = new Dictionary<ChestItem, int>();
        //     foreach (var item in currentlyHeldItems)
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

        #endregion

        #region Enemy Died Events

        public void OnEnemyDied(GameObject enemy)
        {
            var randomChance = Random.Range(0, 100);
            if (randomChance < 10)
            {
                var position = enemy.transform.position;
                var pos = new Vector3(position.x, 0f, position.z);
                var healthPack = Instantiate(HealthPackPrefab, pos, Quaternion.identity);
            }

            AccountManager.instance.statistics.totalKills++;

            // get all boss enemy achievements
            List<Achievement> bossEnemyAchievements = AccountManager.instance.achievementSave.achievements
                .Where(a => a.name == AchievementName.Kill1000Enemies ||
                            a.name == AchievementName.Kill10000Enemies ||
                            a.name == AchievementName.Kill100000Enemies ||
                            a.name == AchievementName.Kill100Enemies).ToList();

            foreach (var a in bossEnemyAchievements)
            {
                if (a.isCompleted) return;
                a.progress++;
                if (a.progress >= a.goal)
                {
                    a.isCompleted = true;
                    AccountManager.instance.AchievementUnlocked(a);
                }
            }
        }

        public void OnBossEnemyDied(GameObject enemy)
        {
            // get all boss enemy achievements
            List<Achievement> bossEnemyAchievements = AccountManager.instance.achievementSave.achievements
                .Where(a => a.name == AchievementName.Kill100Bosses ||
                            a.name == AchievementName.Kill1000Bosses ||
                            a.name == AchievementName.Kill1000Enemies ||
                            a.name == AchievementName.Kill10000Enemies ||
                            a.name == AchievementName.Kill100000Enemies ||
                            a.name == AchievementName.Kill100Enemies).ToList();

            foreach (var a in bossEnemyAchievements)
            {
                if (a.isCompleted) return;
                a.progress++;
                if (a.progress >= a.goal)
                {
                    a.isCompleted = true;
                    AccountManager.instance.AchievementUnlocked(a);
                }
            }

            AccountManager.instance.statistics.totalBossKills++;
            AccountManager.instance.statistics.totalKills++;
        }

        #endregion
    }
}