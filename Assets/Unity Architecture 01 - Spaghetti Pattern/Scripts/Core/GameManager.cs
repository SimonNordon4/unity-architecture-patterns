using UnityEngine;
using UnityEngine.SceneManagement;
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

        [Header("Scenes")]
        public int mainMenuScene = 0;
        public int gameScene = 1;
        public int gameWon = 2;
        public int gameLost = 3;
        public int dungeonScene = 4;

        private GameObject _pauseMenu;
        private GameObject _hudMenu;

        [Header("Settings")]
        public bool showDamageNumbers = true;
        public bool showEnemyHealthBars = true;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        [Header("Health Packs")] 
        public GameObject healthPackPrefab;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (isGameActive)
            {
                roundTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.F)) TogglePauseGame();
        }

#region GamePlay

        public void StartNewGame()
        {
            LoadEnvironment();
            isGameActive = true;
            roundTime = 0f;
            playerManager.playerCurrentHealth = (int)playerManager.playerMaxHealth.value;
            // clear all items
            AudioManager.Instance.OnStartGame();
        }

        public void TogglePauseGame()
        {
            Console.Log("GameManager.TogglePauseGame()", LogFilter.Game, this);
            // If the game is active and we're not already paused.
            if (isGameActive && !isPaused)
            {
                isGameActive = false;
                isPaused = true;
                _pauseMenu.SetActive(true);
            }
            // If the game is not active and we are paused.
            else if (!isGameActive && isPaused)
            {
                isGameActive = true;
                isPaused = false;
                _hudMenu.SetActive(true);
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
            AudioManager.Instance.StopMusic();
            LoadGameWon();
        }

        public void LoseGame()
        {
            Console.Log("GameManager.LoseGame()", LogFilter.Game, this);
            AudioManager.Instance.StopMusic();
            isGameActive = false;
            LoadGameLost();
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
                var healthPack = Instantiate(healthPackPrefab, pos, Quaternion.identity);
            }
        }

#endregion

#region Scene Managment
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }

    public void LoadGame() 
    {
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    public void LoadGameWon()
    {
        SceneManager.LoadScene(gameWon, LoadSceneMode.Single);
    }

    public void LoadGameLost()
    {
        SceneManager.LoadScene(gameLost, LoadSceneMode.Single);
    }

    public void LoadEnvironment()
    {
        SceneManager.LoadScene(dungeonScene, LoadSceneMode.Additive);
    }
#endregion

#region Settings
        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            SaveSettings();
        }

        public void SetShowEnemyHealthBars(bool show)
        {
            showEnemyHealthBars = show;
            // get every enemy in the scene
            var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.SetHealthBarVisibility(showEnemyHealthBars);
            }
            SaveSettings();
        }

        public void ShowDamageNumbers(bool show)
        {
            showDamageNumbers = show;
            SaveSettings();
        }

        public void SaveSettings()
        {
            // TODO
        }

        public void LoadSettings()
        {
            // TODO
        }
#endregion

    }
}