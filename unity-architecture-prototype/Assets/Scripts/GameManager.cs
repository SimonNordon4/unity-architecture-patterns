using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
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

        [Header("Round")]
        public float roundDuration = 20f;
        private float _roundTime;

        public bool isGameActive = false;
        public Vector2 levelBounds = new Vector2(25f, 25f);

        [Header("UI")] 
        public TextMeshProUGUI roundTimeText;
        public GameObject mainMenu;
        public GameObject pauseMenu;
        public GameObject gameMenu;
        public GameObject gameOverMenu;
        public GameObject winMenu;


        private void Start()
        {
            GoToMainMenu();
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
                isGameActive = !isGameActive;
                pauseMenu.SetActive(!isGameActive);
            }
        }

        public void StartNewGame()
        {
            Debug.Log("Start New Game");
            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);
            isGameActive = true;
            _roundTime = 0f;
        }

        public void WinGame()
        {
            Debug.Log("Game Won!");
            mainMenu.SetActive(false);
            gameMenu.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
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
            mainMenu.SetActive(true);
            gameMenu.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);
            isGameActive = false;
            ResetGame();
        }

        public void LoseGame()
        {
            Debug.Log("Lose Game");
            mainMenu.SetActive(false);
            gameMenu.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(true);
            winMenu.SetActive(false);
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
    }
}