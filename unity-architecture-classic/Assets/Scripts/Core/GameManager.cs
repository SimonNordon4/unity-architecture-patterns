using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Classic.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{
    public Stats stats;
    public Inventory inventory;
    
    public UnityEvent tempGameWon = new();
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


    [Header("UI")] 
    public TextMeshProUGUI waveText;
    public List<TextMeshProUGUI> GoldTexts = new();
    public List<TextMeshProUGUI> GoldSubTexts = new();


    [Header("Item UI")] 
    public RectTransform itemHoverImageContainer;
    private readonly List<GameObject> _itemHoverImages = new();
    public ChestItemHoverImage itemHoverImagePrefab;


    public int pityLuck;
    public float pityLuckScaling = 1f;

    public WasdButtonSelector _chestItemsWasdSelector;


    #endregion

    #region Unity Functions

    private void Start()
    {
        // GoToMainMenu();
        // PopulateStats();
        // PopulateStatsUI();
        // LoadStoreItemsIntoStats();
        // playerCurrentHealth = (int)playerMaxHealth.value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) TogglePauseGame();
    }

    #endregion

    #region Application State

    public void StartNewGame()
    {
        Debug.Log("Start New Game");
        AccountManager.instance.statistics.gamesPlayed++;
        isGameActive = true;
        roundTime = 0f;
        //LoadStoreItemsIntoStats();
        // clear all items
        //inventory.ClearAll();
        UpdateItemUI();
        TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Wasd, 2f);
        TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Chest, 12f);
        TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Dash, 22f);
        TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Pause, 28f);
        AudioManager.instance.OnStartGame();
    }

    public void GoToMainMenu()
    {
        AccountManager.instance.Save();
        isGameActive = false;
        isPaused = false;
        ResetGame();
    }

    public void TogglePauseGame()
    {
        // If the game is active and we're not already paused.
        if (isGameActive && !isPaused)
        {
            isGameActive = false;
            isPaused = true;
        }
        // If the game is not active and we are paused.
        else if (!isGameActive && isPaused)
        {
            isGameActive = true;
            isPaused = false;
        }
        // Do nothing otherwise.
    }

    public void ResetGame()
    {
        pityLuck = 0;
        isPaused = false;
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

        // Remove all chests.
        var chests = FindObjectsOfType<Chest>();
        foreach (var chest in chests) Destroy(chest.gameObject);

        // Remove all healthpacks
        var healthPacks = FindObjectsOfType<HealthPackController>();
        foreach (var healthPack in healthPacks) Destroy(healthPack.gameObject);
    }

    public void WinGame()
    {
        tempGameWon.Invoke();
        AccountManager.instance.Save();
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

        AddGoldWhenGameEnds();
        AudioManager.instance.StopMusic();
    }

    public void LoseGame()
    {
        AudioManager.instance.StopMusic();
        Debug.Log("Lose Game");
        isGameActive = false;

        AddGoldWhenGameEnds();

        TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Buy, 2f);
    }

    private void AddGoldWhenGameEnds()
    {
        // get the enemy manager
        var enemyManager = FindObjectOfType<EnemyManager>();

        var totalGold = Mathf.RoundToInt(enemyManager.WaveDatas.Sum(data => data.currentGold + data.bonusGold));

        AccountManager.instance.AddGold(totalGold);

        foreach (var txt in GoldTexts) txt.text = $"+{totalGold}G";
        foreach (var txt in GoldSubTexts)
        {
            txt.text = $"Total: {AccountManager.instance.totalGold}G";
        }

        List<Achievement> achievements = AccountManager.instance.achievementSave.achievements
            .Where(a => a.name == AchievementName.Earn100Gold ||
                        a.name == AchievementName.Earn1000Gold ||
                        a.name == AchievementName.Earn10000Gold ||
                        a.name == AchievementName.Earn100000Gold).ToList();
        Debug.Log("Achievements Found:" + achievements.Count);

        foreach (var a in achievements)
        {
            if (a.isCompleted) continue;
            a.progress += totalGold;
            if (a.progress >= a.goal)
            {
                a.isCompleted = true;
                AccountManager.instance.AchievementUnlocked(a);
            }
        }
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

    #region Chest Creation


    private IEnumerator WaitOneFrameToUnpause()
    {
        yield return new WaitForEndOfFrame();
        isGameActive = true;
    }

    private void UpdateItemUI()
    {
        foreach (var item in _itemHoverImages) Destroy(item);
        _itemHoverImages.Clear();

        // Create a dictionary of every item and how many of them there are.
        var itemDictionary = new Dictionary<ChestItem, int>();
        foreach (var item in inventory.chestItems)
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