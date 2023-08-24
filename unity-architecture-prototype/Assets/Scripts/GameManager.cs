using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

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

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            mainMenu.SetActive(true);
            gameMenu.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);
        }

        private void Update()
        {
            if (isGameActive)
            {
                _roundTime += Time.deltaTime;
                roundTimeText.text = $"Round Time: {(int)(roundDuration)}";
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isGameActive = !isGameActive;
                pauseMenu.SetActive(!isGameActive);
            }

        }

        public void StartNewGame()
        {
            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
            isGameActive = true;
            _roundTime = 0f;
        }
    }
}