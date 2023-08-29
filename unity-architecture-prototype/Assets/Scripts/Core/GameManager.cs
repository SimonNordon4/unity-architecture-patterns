using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;


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
    private float _roundTime;
    public bool isPaused = false;
    public bool isGameActive = false;
    public Vector2 levelBounds = new Vector2(25f, 25f);


    [Header("Stats")] public int playerCurrentHealth = 10;
    public Stat playerMaxHealth = new(10);
    public Stat playerSpeed = new(5);
    public Stat pistolDamage = new(1);
    public Stat pistolRange = new(5);
    public Stat pistolFireRate = new(0.5f);
    public Stat pistolKnockBack = new(1);
    public Stat swordDamage = new(3);
    public Stat swordRange = new(1);
    public Stat swordAttackSpeed = new(3);
    public Stat swordKnockBack = new(3);
    public Stat enemySpawnRate = new(1);
    public Stat healthPackSpawnRate = new(5);
    private readonly Dictionary<StatType, Stat> _stats = new();
    public readonly List<ChestItem> currentlyHeldItems = new();

    [Header("UI")] public TextMeshProUGUI roundTimeText;
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;
    
    public List<TextMeshProUGUI> GoldTexts = new();
    
    [Header("Stat UI")]
    public RectTransform StatContainer;
    private Dictionary<StatType,TextMeshProUGUI> statTexts = new();
    
    [Header("Item UI")]
    public RectTransform itemHoverImageContainer;
    private readonly List<GameObject> _itemHoverImages = new();
    public ChestItemHoverImage itemHoverImagePrefab;

    
    // Chest Item
    public GameObject chestItemMenu;
    public RectTransform chestItemButtonContainer;
    public ChestItemButton chestItemButtonPrefab;
    private readonly List<ChestItemButton> _chestItemButtons = new();

    [Header("Chests")] public Vector2 chestBounds = new Vector2(20f, 20f);
    public List<ChestItem> chestItems;
    public float miniChestCooldown = 15f;
    private float _timeSinceLastMiniChest = 0.0f;
    private bool _nextMiniChest = false;
    public Chest miniChestPrefab;
    public Chest mediumChestPrefab;
    public Chest largeChestPrefab;
    
    [Header("Health Packs")]
    public GameObject HealthPackPrefab;
    
    #endregion

    #region Unity Functions
    private void Start()
    {
        GoToMainMenu();
        PopulateStats();
        PopulateStatsUI();
        LoadStoreItemsIntoStats();
        
        // start the game with one mini chest on the map.
        SpawnMiniChest();

        // Chest has to spawn inside the level.
        if (chestBounds.magnitude > levelBounds.magnitude)
            chestBounds = levelBounds;
    }
    
    private void Update()
    {
        if (isGameActive)
        {
            _roundTime += Time.deltaTime;
            roundTimeText.text = $"Round Time: {(int)(_roundTime)}";

            if (_nextMiniChest)
            {
                _timeSinceLastMiniChest += Time.deltaTime;
                if(_timeSinceLastMiniChest > miniChestCooldown)
                {
                    SpawnMiniChest();
                    _timeSinceLastMiniChest = 0.0f;
                    _nextMiniChest = false;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }
    }
    #endregion

    #region Application State

    public void StartNewGame()
    {
        Debug.Log("Start New Game");
        HideAll();
        gameMenu.SetActive(true);
        isGameActive = true;
        _roundTime = 0f;
        playerCurrentHealth = (int)playerMaxHealth.value;
        LoadStoreItemsIntoStats();
    }
    
    public void GoToMainMenu()
    {
        Debug.Log("Go to Main Menu");
        HideAll();
        mainMenu.SetActive(true);
        isGameActive = false;
        isPaused = false;
        ResetGame();
    }
    
    public void TogglePauseGame()
    {
        // If the game is active and we're not already paused.
        if(isGameActive && !isPaused)
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
        foreach (var projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }

        // find all objects with tag "Spawn Indicator" and destroy them.
        var spawnIndicators = GameObject.FindGameObjectsWithTag("Spawn Indicator");
        foreach (var spawnIndicator in spawnIndicators)
        {
            Destroy(spawnIndicator);
        }
        
        foreach(var val in _stats.Values)
        {
            val.Reset();
        }
        
        // Remove all chests.
        var chests = FindObjectsOfType<Chest>();
        foreach (var chest in chests)
        {
            Destroy(chest.gameObject);
        }
    }
    
    public void WinGame()
    {
        Debug.Log("Game Won!");
        HideAll();
        winMenu.SetActive(true);
        isGameActive = false;
        _roundTime = 0f;
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
        AccountManager.instance.AddGold(enemyManager.totalEnemiesKilled);

        foreach (var txt in GoldTexts)
        {
            txt.text = $"Gold Added: {enemyManager.totalEnemiesKilled}";
        }
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
        _stats.Add(StatType.PistolDamage, pistolDamage);
        _stats.Add(StatType.PistolRange, pistolRange);
        _stats.Add(StatType.PistolFireRate, pistolFireRate);
        _stats.Add(StatType.PistolKnockBack, pistolKnockBack);
        _stats.Add(StatType.SwordDamage, swordDamage);
        _stats.Add(StatType.SwordRange, swordRange);
        _stats.Add(StatType.SwordAttackSpeed, swordAttackSpeed);
        _stats.Add(StatType.SwordKnockBack, swordKnockBack);
        _stats.Add(StatType.EnemySpawnRate, enemySpawnRate);
        _stats.Add(StatType.HealthPackSpawnRate, healthPackSpawnRate);
        playerCurrentHealth = (int)playerMaxHealth.value;
    }

    private void PopulateStatsUI()
    {
        foreach (var key in _stats.Keys)
        {
            var newStatText = Instantiate(new GameObject(key.ToString()).AddComponent<TextMeshProUGUI>(), StatContainer);
            newStatText.fontSize = 32;
            // set the width of the text transform to 400
            newStatText.rectTransform.sizeDelta = new Vector2(400, 32);
            statTexts.Add(key, newStatText);
        }
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        foreach (var key in statTexts.Keys)
        {
            var statTypeText = key.ToString();
            for (var i = 1; i < statTypeText.Length; i++)
            {
                if (char.IsUpper(statTypeText[i]))
                {
                    statTypeText = statTypeText.Insert(i, " ");
                    i++;
                }
            }
            
            var newStatText = statTexts[key];
            newStatText.text = $"{statTypeText}: {_stats[key].value}";
            // check if the stat value is above, below or equal to the initial value.
            if (_stats[key].value > _stats[key].initialValue)
            {
                newStatText.color = new Color(0.75f, 1f, 0.75f);
            }
            else if (_stats[key].value < _stats[key].initialValue)
            {
                newStatText.color = new Color(1f, 0.75f, 0.75f);
            }
            else
            {
                newStatText.color = new Color(0.8f, 0.8f, 0.8f);
            }
        }
    }

    public void LoadStoreItemsIntoStats()
    {
        Debug.Log("Loading Store items!");
        foreach (var stat in _stats.Values)
        {
            stat.Reset();
        }
        var storeItems = AccountManager.instance.storeItems;
        foreach (var store in storeItems)
        {
            var currentModifier = store.tierModifiers[store.currentTier];
            var stat = _stats[currentModifier.statType];
            stat.AddModifier(currentModifier);
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
        miniChest.minTier = 1;
        miniChest.maxTier = 1;
    }

    public void SpawnMediumChest()
    {
        var mediumChest = Instantiate(mediumChestPrefab,GetRandomChestPosition(), Quaternion.identity);
        mediumChest.chestType = ChestType.Medium;
        mediumChest.minTier = 2;
        mediumChest.maxTier = 3;
    }

    public void SpawnLargeChest()
    {
        var largeChest = Instantiate(largeChestPrefab,GetRandomChestPosition(), Quaternion.identity);
        largeChest.chestType = ChestType.Large;
        largeChest.minTier = 3;
        largeChest.maxTier = 5;
    }
    
    public void PickupChest(Chest chest)
    {
        // If we pickup a mini chest, we can start spawning the next mini chest.
        if (chest.chestType == ChestType.Mini)
        {
            _nextMiniChest = true;
        }
        isGameActive = false;
        HideAll();
        chestItemMenu.SetActive(true);

        foreach (var cib in _chestItemButtons)
        {
            Destroy(cib.gameObject);
        }
        _chestItemButtons.Clear();
        
        // we wanted a weight average between 2 - 5 items spawning, with odds being increased by luck, which will be added later.
        var itemsChance = Random.Range(0, 100);
        int numberOfItems = 0;

        switch (itemsChance)
        {
            case >98:
                numberOfItems = 5;
                break;
            case >90:
                numberOfItems = 4;
                break;
            case >75:
                numberOfItems = 3;
                break;
            default:
                numberOfItems = 2;
                break;
        }

        for (var i = 0; i < numberOfItems; i++)
        {
            var newChestItemButton = Instantiate(chestItemButtonPrefab, chestItemButtonContainer);
            _chestItemButtons.Add(newChestItemButton);
            
            // Collect all items with a tier equal to or less than the chest tier
            var possibleItems = new List<ChestItem>();
            foreach (var chestItem in chestItems)
            {
                if (chestItem.tier >= chest.minTier && chestItem.tier <= chest.maxTier)
                {
                    possibleItems.Add(chestItem);
                }
            }
            
            // Now randomly select one of these possible items based on its probabilty
            var totalSpawnChance = 0;
            foreach (var chestItem in possibleItems)
            {
                totalSpawnChance += chestItem.spawnChance;
            }
            
            var randomSpawnChance = Random.Range(0, totalSpawnChance);
            var currentSpawnChance = 0;
            for (var j = 0; j < possibleItems.Count; j++)
            {
                var x = j;
                currentSpawnChance += possibleItems[x].spawnChance;

                if (randomSpawnChance >= currentSpawnChance) continue;
                // We have found the item to spawn
                var item = possibleItems[x];
                newChestItemButton.Initialize(item);
                break;
            }
        }


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

            // If it's a max health mod, we need to also increase the current health.
            if (mod.statType == StatType.PlayerHealth)
            {
                playerCurrentHealth += (int)mod.modifierValue;
            }
        }
        
        UpdateStatsUI();
        UpdateItemUI();
        isGameActive = true;
    }

    private void UpdateItemUI()
    {
        Debug.Log("Updating Item UI");
        foreach (var item in _itemHoverImages)
        {
            Destroy(item);
        }
        _itemHoverImages.Clear();
        
        // Create a dictionary of every item and how many of them there are.
        var itemDictionary = new Dictionary<ChestItem, int>();
        foreach (var item in currentlyHeldItems)
        {
            if (itemDictionary.ContainsKey(item))
            {
                itemDictionary[item]++;
            }
            else
            {
                itemDictionary.Add(item, 1);
            }
        }
        
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
        if(randomChance < healthPackSpawnRate.value)
        {
            var healthPack = Instantiate(HealthPackPrefab, enemy.transform.position, Quaternion.identity);
        }
    }

    public void OnBossEnemyDied(GameObject enemy)
    {
        
    }
    
    #endregion
}