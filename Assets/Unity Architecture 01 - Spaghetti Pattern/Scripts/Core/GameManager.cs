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


        [Header("Health Packs")] 
        public GameObject HealthPackPrefab;

        #endregion

        #region Unity Functions

        private void Start()
        {
            playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None)[0];
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;

            
            isGameActive = true;
            StartNewGame();
        }

        private void Update()
        {
            if (isGameActive)
            {
                roundTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.F)) TogglePauseGame();
        }

        #endregion

        #region Application State

        public void StartNewGame()
        {
            Debug.Log("Start New Game");
            HideAll();
            AccountManager.Instance.statistics.gamesPlayed++;
            hudMenu.SetActive(true);
            isGameActive = true;
            roundTime = 0f;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
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
            AccountManager.Instance.Save();
            HideAll();
            winMenu.SetActive(true);
            isGameActive = false;
            AccountManager.Instance.statistics.gamesWon++;

            // check if faster time
            if (AccountManager.Instance.statistics.fastestWin == 0 ||
                roundTime < AccountManager.Instance.statistics.fastestWin)
                AccountManager.Instance.statistics.fastestWin = roundTime;

            List<Achievement> achievements = AccountManager.Instance.achievementSave.achievements
                .Where(a => a.name == AchievementName.BeatTheGame ||
                            a.name == AchievementName.BeatTheGame10Times).ToList();
            foreach (var a in achievements)
            {
                if (a.isCompleted) return;
                a.progress++;
                if (a.progress >= a.goal)
                {
                    a.isCompleted = true;
                    AccountManager.Instance.AchievementUnlocked(a);
                }
            }

            var under1hour = AccountManager.Instance.achievementSave.achievements
                .First(x => x.name == AchievementName.WinInUnder1Hour);

            if (!under1hour.isCompleted)
            {
                if (under1hour.progress > roundTime)
                    under1hour.progress = (int)roundTime;
                under1hour.isCompleted = roundTime < under1hour.goal;
            }


            var under45mins = AccountManager.Instance.achievementSave.achievements
                .First(x => x.name == AchievementName.WinInUnder45Minutes);

            if (!under45mins.isCompleted)
            {
                if (under45mins.progress > roundTime)
                    under45mins.progress = (int)roundTime;
                under45mins.isCompleted = roundTime < under45mins.goal;
            }


            var under30mins = AccountManager.Instance.achievementSave.achievements
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
            AccountManager.Instance.Save();

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

            AccountManager.Instance.statistics.totalKills++;

            // get all boss enemy achievements
            List<Achievement> bossEnemyAchievements = AccountManager.Instance.achievementSave.achievements
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
                    AccountManager.Instance.AchievementUnlocked(a);
                }
            }
        }

        public void OnBossEnemyDied(GameObject enemy)
        {
            // get all boss enemy achievements
            List<Achievement> bossEnemyAchievements = AccountManager.Instance.achievementSave.achievements
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
                    AccountManager.Instance.AchievementUnlocked(a);
                }
            }

            AccountManager.Instance.statistics.totalBossKills++;
            AccountManager.Instance.statistics.totalKills++;
        }

        #endregion
    }
}