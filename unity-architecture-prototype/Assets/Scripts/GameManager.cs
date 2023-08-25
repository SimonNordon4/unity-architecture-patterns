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

    [Header("Round")] public float roundDuration = 20f;
    private float _roundTime;

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
    private readonly Dictionary<StatType, Stat> _stats = new();
    public readonly List<ChestItem> currentlyHeldItems = new();

    [Header("UI")] public TextMeshProUGUI roundTimeText;
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;
    public GameObject chestItemMenu;
    public RectTransform chestItemButtonContainer;
    public ChestItemButton chestItemButtonPrefab;
    private readonly List<ChestItemButton> _chestItemButtons = new();

    [Header("Chests")]
    public List<ChestItem> chestItems;

    private void Start()
    {
        GoToMainMenu();
        PopulateStats();
    }

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
        playerCurrentHealth = (int)playerMaxHealth.value;
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

    private void Update()
    {
        if (isGameActive)
        {
            _roundTime += Time.deltaTime;
            roundTimeText.text = $"Round Time: {(int)(_roundTime)}";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }
    }

    public void TogglePauseGame()
    {
        HideAll();
        isGameActive = !isGameActive;
        pauseMenu.SetActive(!isGameActive);
        gameMenu.SetActive(isGameActive);
    }

    public void StartNewGame()
    {
        Debug.Log("Start New Game");
        HideAll();
        gameMenu.SetActive(true);
        isGameActive = true;
        _roundTime = 0f;
    }

    public void WinGame()
    {
        Debug.Log("Game Won!");
        HideAll();
        winMenu.SetActive(true);
        isGameActive = false;
        _roundTime = 0f;
    }

    public void ResetGame()
    {
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
    }

    public void GoToMainMenu()
    {
        Debug.Log("Go to Main Menu");
        HideAll();
        mainMenu.SetActive(true);
        isGameActive = false;
        ResetGame();
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        HideAll();
        gameOverMenu.SetActive(true);
        isGameActive = false;
    }

    public void QuitApplication()
    {
        Debug.Log("Quit Application");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    public void CreateChest(int chestTier)
    {
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
                if (chestItem.tier <= chestTier)
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
        Debug.Log("Applying Item " + item.name);
        HideAll();
        gameMenu.SetActive(true);

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
        
        // apply the item to the correct stat based on it's stat type
        
        
        isGameActive = true;
    }
}