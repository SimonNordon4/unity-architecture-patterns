using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace UnityArchitecture.SpaghettiPattern
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<GameManager>();
                return _instance;
            }
        }

        public PlayerManager playerManager;

        [Header("Round")]
        public float roundTime;
        public bool isPaused = false;
        public bool isGameActive = false;
        public Vector2 levelBounds = new(25f, 25f);

        [Header("UI")]
        public GameObject pauseMenu;
        public GameObject hudMenu;

        [Header("Health Packs")] 
        public GameObject HealthPackPrefab;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SceneManager.Instance.LoadEnvironment();
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

        public void StartNewGame()
        {
            Console.Log("GameManager.StartNewGame()", LogFilter.Game, this);
            AccountManager.Instance.statistics.gamesPlayed++;
            hudMenu.SetActive(true);
            isGameActive = true;
            roundTime = 0f;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            // clear all items

            // UpdateItemUI();
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Wasd, 2f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Chest, 12f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Dash, 22f);
            TutorialManager.instance.ShowTip(TutorialManager.TutorialMessage.Pause, 28f);
            AudioManager.instance.OnStartGame();
        }

        public void TogglePauseGame()
        {
            Console.Log("GameManager.TogglePauseGame()", LogFilter.Game, this);
            // If the game is active and we're not already paused.
            if (isGameActive && !isPaused)
            {
                isGameActive = false;
                isPaused = true;
                pauseMenu.SetActive(true);
            }
            // If the game is not active and we are paused.
            else if (!isGameActive && isPaused)
            {
                isGameActive = true;
                isPaused = false;
                hudMenu.SetActive(true);
            }
            // Do nothing otherwise.
        }

        public void ResetGame()
        {
            Console.Log("GameManager.ResetGame()", LogFilter.Game, this);
            isPaused = false;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            var playerController = FindFirstObjectByType<PlayerManager>();
            playerController.ResetPlayer();

            var enemyManager = FindFirstObjectByType<EnemyManager>();
            enemyManager.Reset();

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
            Console.Log("GameManager.WinGame()", LogFilter.Game, this);
            AccountManager.Instance.Save();
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
            SceneManager.Instance.LoadGameWon();
        }

        public void LoseGame()
        {
            Console.Log("GameManager.LoseGame()", LogFilter.Game, this);
            AudioManager.instance.StopMusic();
            isGameActive = false;
            SceneManager.Instance.LoadGameLost();
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
    }
}