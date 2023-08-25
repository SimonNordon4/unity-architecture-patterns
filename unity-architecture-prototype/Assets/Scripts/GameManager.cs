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

    [Header("Chests")] public List<ChestItem> chestItems;


    private void Start()
    {
        GoToMainMenu();
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
        isGameActive = true;
    }
}