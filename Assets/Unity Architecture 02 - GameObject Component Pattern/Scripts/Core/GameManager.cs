using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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

        [FormerlySerializedAs("playerManager")] public PlayerController playerController;

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

        [Header("UI")]
        public GameObject hudMenu;
        public GameObject pauseMenu;
        public GameObject settingsMenu;

        [Header("Settings")]
        public bool showDamageNumbers = true;
        public bool showEnemyHealthBars = true;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        [Header("Health Packs")] 
        public GameObject healthPackPrefab;

        private void Awake()
        {
            // If the scene is the game scene, this should become the new GameManager.
            if (SceneManager.GetActiveScene().buildIndex == gameScene)
            {
                if (_instance != null)
                {
                    Destroy(_instance); 
                }
                
                _instance = this;
            }
            else
            {
                // If this is a random scene, and the game manager already exists, destroy this instance.
                if (_instance != null)
                {
                    Destroy(this);
                }
                else
                {
                    _instance = this;
                }
            }
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            // If the scene is the game scene, this should become the new GameManager.
            if (SceneManager.GetActiveScene().buildIndex == gameScene)
            {
                StartNewGame();
            }
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
            playerController.playerCurrentHealth = (int)playerController.playerMaxHealth.value;
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
                pauseMenu.SetActive(true);
                hudMenu.SetActive(false);
                Time.timeScale = 0f;
            }
            // If the game is not active and we are paused.
            else if (!isGameActive && isPaused)
            {
                isGameActive = true;
                isPaused = false;
                hudMenu.SetActive(true);
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
            // Do nothing otherwise.
        }

        public void ShowSettingsMenu()
        {
            settingsMenu.SetActive(true);
            pauseMenu.SetActive(false);
            hudMenu.SetActive(false);
        }

        public void HideSettingsMenu()
        {
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            hudMenu.SetActive(false);
        }

        public void ResetGame()
        {
            Console.Log("GameManager.ResetGame()", LogFilter.Game, this);
            isPaused = false;
            this.playerController.playerCurrentHealth = (int)this.playerController.playerMaxHealth.value;
            var playerController = FindFirstObjectByType<PlayerController>();
            playerController.ResetPlayer();

            var enemyManager = FindFirstObjectByType<EnemyManager>();
            enemyManager.Reset();

            var projectiles = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
            foreach (var projectile in projectiles) Destroy(projectile.gameObject);

            // find all objects with tag "Spawn Indicator" and destroy them.
            var spawnIndicators = GameObject.FindGameObjectsWithTag("Spawn Indicator");
            foreach (var spawnIndicator in spawnIndicators) Destroy(spawnIndicator);

            this.playerController.ResetStats();

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

        public void EnemyDied(EnemyController enemy)
        {
            var healthPackChance = (1 - playerController.playerCurrentHealth / playerController.playerMaxHealth.value) * 0.15f;
            var rollChance = Random.Range(0f, 1f);

            // Max 15% chance to spawn a hp pack.
            if (rollChance < healthPackChance)
            {
                var healthPack = Instantiate(healthPackPrefab);
                var groundPos = new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z);
                healthPack.transform.position = groundPos;
            }
        }


        public void QuitApplication()
        {
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
        UnloadEnvironment();
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }

    public void LoadGame() 
    {
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    public void LoadGameWon()
    {
        UnloadEnvironment();
        SceneManager.LoadScene(gameWon, LoadSceneMode.Single);
    }

    public void LoadGameLost()
    {
        UnloadEnvironment();
        SceneManager.LoadScene(gameLost, LoadSceneMode.Single);
    }

    public void LoadEnvironment()
    {
        if (!SceneManager.GetSceneByBuildIndex(dungeonScene).isLoaded)
        {
            Debug.Log("Loading Environment");
            SceneManager.LoadScene(dungeonScene, LoadSceneMode.Additive);
        }
    }

    private void UnloadEnvironment()
    {
        if (SceneManager.GetSceneByBuildIndex(dungeonScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(dungeonScene);
        }
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
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
            PlayerPrefs.SetInt("ShowEnemyHealthBars", showEnemyHealthBars ? 1 : 0);
            PlayerPrefs.SetInt("ShowDamageNumbers", showDamageNumbers ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
            showEnemyHealthBars = PlayerPrefs.GetInt("ShowEnemyHealthBars", 1) == 1;
            showDamageNumbers = PlayerPrefs.GetInt("ShowDamageNumbers", 1) == 1;
        }
#endregion

    }
}