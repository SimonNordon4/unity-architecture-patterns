using UnityArchitecture.GameObjectComponentPattern;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace UnityArchitecture.GameObjectComponentPattern
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
        public bool isPaused = false;
        public bool isGameActive = false;
        public Vector2 levelBounds = new Vector2(10, 10);
        public float roundTime = 0;
        

        [Header("UI")]
        public GameObject hudMenu;
        public GameObject pauseMenu;
        public GameObject settingsMenu;


        [Header("Health Packs")] 
        public GameObject healthPackPrefab;

        private void Awake()
        {
            // If the scene is the game scene, this should become the new GameManager.
            if (SceneLoader.Instance.CurrentMainScene == SceneLoader.Instance.gameScene)
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
            if (SceneLoader.Instance.CurrentMainScene == SceneLoader.Instance.gameScene)
            {
                StartNewGame();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) TogglePauseGame();
        }

#region GamePlay

        public void StartNewGame()
        {
            SceneLoader.Instance.LoadEnvironment();
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

            var projectiles = FindObjectsByType<Old_Projectile>(FindObjectsSortMode.None);
            foreach (var projectile in projectiles) Destroy(projectile.gameObject);

            // find all objects with tag "Spawn Indicator" and destroy them.
            var spawnIndicators = GameObject.FindGameObjectsWithTag("Spawn Indicator");
            foreach (var spawnIndicator in spawnIndicators) Destroy(spawnIndicator);

            this.playerController.ResetStats();

            // Remove all chests.
            var chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
            foreach (var chest in chests) Destroy(chest.gameObject);

            // Remove all healthpacks
            var healthPacks = FindObjectsByType<HealthPack>(FindObjectsSortMode.None);
            foreach (var healthPack in healthPacks) Destroy(healthPack.gameObject);
        }

        public void WinGame()
        {
            Console.Log("GameManager.WinGame()", LogFilter.Game, this);

            isGameActive = false;

            AudioManager.Instance.StopMusic();
            SceneLoader.Instance.LoadGameWon();
        }

        public void LoseGame()
        {
            Console.Log("GameManager.LoseGame()", LogFilter.Game, this);
            AudioManager.Instance.StopMusic();
            isGameActive = false;
            SceneLoader.Instance.LoadGameLost();
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


    }
}