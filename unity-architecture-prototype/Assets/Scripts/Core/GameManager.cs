using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    #region Variables

    [Header("Round")] public float roundDuration = 20f;
    public float roundTime;
    public bool isPaused = false;
    public bool isGameActive = false;
    public Vector2 levelBounds = new(25f, 25f);


    [Header("Stats")] public int playerCurrentHealth = 10;
    public Stat playerMaxHealth = new(10);
    public Stat playerSpeed = new(5);
    public Stat block = new(0);

    public Stat pistolDamage = new(1);
    public Stat pistolRange = new(5);
    public Stat pistolFireRate = new(0.5f);
    public Stat pistolKnockBack = new(1);
    public Stat pistolPierce = new(0);

    public Stat swordDamage = new(3);
    public Stat swordRange = new(1);
    public Stat swordAttackSpeed = new(3);
    public Stat swordKnockBack = new(3);
    public Stat swordArc = new(45);

    public Stat luck = new(0);
    public Stat enemySpawnRate = new(1);
    public Stat healthPackSpawnRate = new(5);


    private readonly Dictionary<StatType, Stat> _stats = new();
    public readonly List<ChestItem> currentlyHeldItems = new();

    [Header("UI")] public TextMeshProUGUI roundTimeText;
    public TextMeshProUGUI waveText;
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    public List<TextMeshProUGUI> GoldTexts = new();

    [Header("Stat UI")] public List<RectTransform> StatContainers = new();
    private Dictionary<StatType,List<TextMeshProUGUI>> statTexts = new();

    [Header("Item UI")] public RectTransform itemHoverImageContainer;
    private readonly List<GameObject> _itemHoverImages = new();
    public ChestItemHoverImage itemHoverImagePrefab;


    // Chest Item
    public GameObject chestItemMenu;
    public RectTransform chestItemButtonContainer;
    public ChestItemButton chestItemButtonPrefab;
    private readonly List<ChestItemButton> _chestItemButtons = new();

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

    public WasdButtonSelector _chestItemsWasdSelector;

    [Header("Health Packs")] public GameObject HealthPackPrefab;

    #endregion

    #region Unity Functions
    
    private void Start()
    {
        GoToMainMenu();
        PopulateStats();
        PopulateStatsUI();
        LoadStoreItemsIntoStats();
        playerCurrentHealth = (int)playerMaxHealth.value;

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
    }

    private void Update()
    {
        if (isGameActive)
        {
            roundTime += Time.deltaTime;
            
            //format round time in MM:SS
            var minutes = Mathf.FloorToInt(roundTime / 60f);
            var seconds = Mathf.FloorToInt(roundTime % 60f);
            roundTimeText.text = $"Round Time: {minutes:00}:{seconds:00}";

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

        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseGame();
    }

    #endregion

    #region Application State

    public void StartNewGame()
    {
        Debug.Log("Start New Game");
        HideAll();
        AccountManager.instance.statistics.gamesPlayed++;
        gameMenu.SetActive(true);
        isGameActive = true;
        roundTime = 0f;
        LoadStoreItemsIntoStats();
        playerCurrentHealth = (int)playerMaxHealth.value;
        SpawnMiniChest();
        // clear all items
        currentlyHeldItems.Clear();
        UpdateItemUI();
        
    }

    public void GoToMainMenu()
    {
        AccountManager.instance.Save();
        HideAll();
        mainMenu.SetActive(true);
        isGameActive = false;
        isPaused = false;
        ResetGame();
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
            gameMenu.SetActive(true);
        }
        // Do nothing otherwise.
    }

    public void ResetGame()
    {
        isPaused = false;
        playerCurrentHealth = (int)playerMaxHealth.value;
        Debug.Log("Reset Game");
        var playerController = FindObjectOfType<PlayerController>();
        playerController.ResetPlayer();

        var enemyManager = FindObjectOfType<EnemyManager>();
        enemyManager.ResetEnemyManager();

        var projectiles = FindObjectsOfType<Projectile>();
        foreach (var projectile in projectiles) Destroy(projectile.gameObject);

        // find all objects with tag "Spawn Indicator" and destroy them.
        var spawnIndicators = GameObject.FindGameObjectsWithTag("Spawn Indicator");
        foreach (var spawnIndicator in spawnIndicators) Destroy(spawnIndicator);

        foreach (var val in _stats.Values) val.Reset();

        // Remove all chests.
        var chests = FindObjectsOfType<Chest>();
        foreach (var chest in chests) Destroy(chest.gameObject);
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
            if(under1hour.progress > roundTime)
                under1hour.progress = (int)roundTime;
            under1hour.isCompleted = roundTime < under1hour.goal;
        }
            
        
        var under45mins = AccountManager.instance.achievementSave.achievements
            .First(x => x.name == AchievementName.WinInUnder45Minutes);

        if (!under45mins.isCompleted)
        {
            if(under45mins.progress > roundTime)
                under45mins.progress = (int)roundTime;
            under45mins.isCompleted = roundTime < under45mins.goal;
        }
           
        
        var under30mins = AccountManager.instance.achievementSave.achievements
            .First(x => x.name == AchievementName.WinInUnder30Minutes);
        
        if(!under30mins.isCompleted)
        {
            if(under30mins.progress > roundTime)
                under30mins.progress = (int)roundTime;
            under30mins.isCompleted = roundTime < under30mins.goal;
        }
        
        roundTime = 0f;
        


        AddGoldWhenGameEnds();
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        HideAll();
        gameOverMenu.SetActive(true);
        isGameActive = false;

        AddGoldWhenGameEnds();
    }

    private void AddGoldWhenGameEnds()
    {
        // get the enemy manager
        var enemyManager = FindObjectOfType<EnemyManager>();

        var totalGold = enemyManager.WaveDatas.Sum(data => data.totalGold);
        
        Debug.Log("Total gold: " + totalGold);
        
        List<Achievement> achievements = AccountManager.instance.achievementSave.achievements
            .Where(a => a.name == AchievementName.Earn100Gold ||
                        a.name == AchievementName.Earn1000Gold ||
                        a.name == AchievementName.Earn10000Gold ||
                        a.name == AchievementName.Earn100000Gold).ToList();
        foreach (var a in achievements)
        {
            if (a.isCompleted) return;
            a.progress += totalGold;
            if (a.progress >= a.goal)
            {
                a.isCompleted = true;
                AccountManager.instance.AchievementUnlocked(a);
            }
        }

        AccountManager.instance.AddGold(totalGold);

        foreach (var txt in GoldTexts) txt.text = $"Gold Added: {totalGold}";
    }

    private void HideAll()
    {
        mainMenu.SetActive(false);
        gameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
        chestItemMenu.SetActive(false);
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

    #region Stats UI

    private void PopulateStats()
    {
        _stats.Add(StatType.PlayerHealth, playerMaxHealth);
        _stats.Add(StatType.PlayerSpeed, playerSpeed);
        _stats.Add(StatType.Block, block);

        _stats.Add(StatType.PistolDamage, pistolDamage);
        _stats.Add(StatType.PistolRange, pistolRange);
        _stats.Add(StatType.PistolFireRate, pistolFireRate);
        _stats.Add(StatType.PistolKnockBack, pistolKnockBack);
        _stats.Add(StatType.PistolPierce, pistolPierce);

        _stats.Add(StatType.SwordDamage, swordDamage);
        _stats.Add(StatType.SwordRange, swordRange);
        _stats.Add(StatType.SwordAttackSpeed, swordAttackSpeed);
        _stats.Add(StatType.SwordKnockBack, swordKnockBack);
        _stats.Add(StatType.SwordArc, swordArc);

        _stats.Add(StatType.Luck, luck);
        _stats.Add(StatType.EnemySpawnRate, enemySpawnRate);
        _stats.Add(StatType.HealthPackSpawnRate, healthPackSpawnRate);
        playerCurrentHealth = (int)playerMaxHealth.value;
    }

    private void PopulateStatsUI()
    {

        foreach (var statContainer in StatContainers)
        {
            // get all the children of the stat container and destroy them.
            foreach (Transform child in statContainer) Destroy(child.gameObject);
            
            foreach (var key in _stats.Keys)
            {
                var newStatText = Instantiate(new GameObject(key.ToString()).AddComponent<TextMeshProUGUI>(),
                    statContainer);
                newStatText.fontSize = 24;
                // set the width of the text transform to 400
                newStatText.rectTransform.sizeDelta = new Vector2(400, 32);

                if (statTexts.ContainsKey(key))
                {
                    statTexts[key].Add(newStatText);
                }
                else
                {
                    statTexts.Add(key, new List<TextMeshProUGUI>(){newStatText});
                }
            }
        }

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        foreach (var key in statTexts.Keys)
        {
            var statTypeText = key.ToString();
            for (var i = 1; i < statTypeText.Length; i++)
                if (char.IsUpper(statTypeText[i]))
                {
                    statTypeText = statTypeText.Insert(i, " ");
                    i++;
                }

            foreach (var newStatText in statTexts[key])
            {
                newStatText.text = $"{statTypeText}: {_stats[key].value}";
                // check if the stat value is above, below or equal to the initial value.
                if (_stats[key].value > _stats[key].initialValue)
                    newStatText.color = new Color(0.75f, 1f, 0.75f);
                else if (_stats[key].value < _stats[key].initialValue)
                    newStatText.color = new Color(1f, 0.75f, 0.75f);
                else
                    newStatText.color = new Color(0.8f, 0.8f, 0.8f);
            }
        }
    }

    public void LoadStoreItemsIntoStats()
    {
        foreach (var stat in _stats.Values) stat.Reset();
        var storeItems = AccountManager.instance.storeItems;
        foreach (var store in storeItems)
        {
            // If the item tier is 0, it hasn't been purchased yet.
            if (store.currentTier == 0) continue;
            var currentModifier = store.tierModifiers[store.currentTier - 1];
            var stat = _stats[currentModifier.statType];
            stat.AddModifier(currentModifier);
            AccountManager.instance.CheckIfHighestStat(currentModifier.statType, stat.value);
        }

        UpdateStatsUI();
    }

    #endregion

    #region Chest Creation

    private Vector3 GetRandomChestPosition()
    {
        return new Vector3(Random.Range(-chestBounds.x, chestBounds.x), 0, Random.Range(-chestBounds.y, chestBounds.y));
    }

    private void SpawnMiniChest()
    {
        var miniChest = Instantiate(miniChestPrefab, GetRandomChestPosition(), Quaternion.identity);
        // Figure out an algorithm for min tier to random between 1-2 based on luck. Always 1 at 0 luck, always 2 at 10 luck.
        var chance = Random.Range(0, 9);
        
        
        miniChest.minTier = chance < luck.value ? 2 : 1;
        miniChest.maxTier = Mathf.Clamp((int)(luck.value * 0.5f), 1, 5);
        Debug.Log("Mini chest max tier: " + miniChest.maxTier);
        
        if(miniChest.maxTier < miniChest.minTier)
            miniChest.maxTier = miniChest.minTier;
    }

    public void PickupChest(Chest chest)
    {
        // monitor stats.
        AccountManager.instance.statistics.totalChestsOpened++;
        
        List<Achievement> achievements = AccountManager.instance.achievementSave.achievements
            .Where(a => a.name == AchievementName.Open100Chests ||
                        a.name == AchievementName.Open1000Chests).ToList();
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

        // If we pickup a mini chest, we can start spawning the next mini chest.
        if (chest.chestType == ChestType.Mini) _nextMiniChest = true;
        isGameActive = false;
        HideAll();
        chestItemMenu.SetActive(true);

        foreach (var cib in _chestItemButtons) Destroy(cib.gameObject);
        _chestItemButtons.Clear();

        // we wanted a weight average between 2 - 5 items spawning, with odds being increased by luck, which will be added later.
        var itemsChance = Random.Range(0, 100);
        var numberOfItems = 0;

        var luckFactor = luck.value * 10f;
        itemsChance += (int)luckFactor;

        switch (itemsChance)
        {
            case > 98:
                numberOfItems = 5;
                break;
            case > 90:
                numberOfItems = 4;
                break;
            case > 75:
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
                newChestItemButton.Initialize(item);
                break;
            }

            // For keyboard input.
            _chestItemsWasdSelector.buttons.Clear();
            foreach (var chestItemUI in _chestItemButtons)
                _chestItemsWasdSelector.buttons.Add(chestItemUI.GetComponent<Button>());
        }
    }

    // Return the tier
    private int GetRandomChestItemTier(Chest chest)
    {
        if (chest.minTier == chest.maxTier)
            return chest.minTier;

        var tier = 0;
        if (luck.value > 10) luck.value = 10;
        var chance = Random.Range(0, 50) + luck.value * 10f;
        
        Debug.Log("Chance: " + chance);

        tier = chance switch
        {
            < 50 => 1,
            < 70 => 2,
            < 80 => 3,
            < 95 => 4,
            _ => 5
        };

        Debug.Log("Pre clamped tier: " + tier);

        tier = Mathf.Clamp(tier, chest.minTier, chest.maxTier);

        Debug.Log("given chest range: " + chest.minTier + " - " + chest.maxTier);
        Debug.Log("Return tier: " + tier);
        
        return tier;
    }

    public void ApplyItem(ChestItem item)
    {
        HideAll();
        gameMenu.SetActive(true);
        currentlyHeldItems.Add(item);

        // Add modifiers to the stats.
        foreach (var mod in item.modifiers)
        {
            var stat = _stats[mod.statType];
            stat.AddModifier(mod);

            // TODO: This might be broken.
            AccountManager.instance.CheckIfHighestStat(mod.statType, stat.value);

            // If it's a max health mod, we need to also increase the current health.
            if (mod.statType == StatType.PlayerHealth)
            {
                AccountManager.instance.statistics.totalDamageHealed += (int)mod.modifierValue;
                playerCurrentHealth += (int)mod.modifierValue;
            }
        }

        UpdateStatsUI();
        UpdateItemUI();
        isGameActive = true;
    }

    private void UpdateItemUI()
    {
        foreach (var item in _itemHoverImages) Destroy(item);
        _itemHoverImages.Clear();

        // Create a dictionary of every item and how many of them there are.
        var itemDictionary = new Dictionary<ChestItem, int>();
        foreach (var item in currentlyHeldItems)
            if (itemDictionary.ContainsKey(item))
                itemDictionary[item]++;
            else
                itemDictionary.Add(item, 1);

        // Create a new item hover image for each item in the dictionary.
        foreach (var item in itemDictionary.Keys)
        {
            var newHoverImage = Instantiate(itemHoverImagePrefab, itemHoverImageContainer);
            newHoverImage.Initialize(item, itemDictionary[item]);
            _itemHoverImages.Add(newHoverImage.gameObject);
        }
    }

    #endregion

    #region Enemy Died Events

    public void OnEnemyDied(GameObject enemy)
    {
        var randomChance = Random.Range(0, 100);
        if (randomChance < healthPackSpawnRate.value)
        {
            var healthPack = Instantiate(HealthPackPrefab, enemy.transform.position, Quaternion.identity);
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