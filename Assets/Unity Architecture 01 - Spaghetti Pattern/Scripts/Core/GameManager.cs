using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            DontDestroyOnLoad(gameObject);
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
            hudMenu.SetActive(true);
            isGameActive = true;
            roundTime = 0f;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            // clear all items
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

            isGameActive = false;

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



            
        }

        public void OnBossEnemyDied(GameObject enemy)
        {

        }
    }
}