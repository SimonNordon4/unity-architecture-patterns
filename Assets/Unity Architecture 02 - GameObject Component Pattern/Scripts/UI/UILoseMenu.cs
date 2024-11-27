using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityArchitecture.GameObjectComponentPattern;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UILoseMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI enemiesKilledText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            timerText.text = $"Time Alive: {Mathf.FloorToInt(GameManager.Instance.roundTime / 60):00}:{Mathf.FloorToInt(GameManager.Instance.roundTime % 60):00}"; 
            enemiesKilledText.text = $"Enemies Killed: {EnemyManager.Instance.totalEnemiesKilled}";

            playAgainButton.onClick.AddListener(LoadGame);
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }

        private void OnDestroy()
        {
            playAgainButton.onClick.RemoveListener(LoadGame);
            mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        }

        public void LoadMainMenu()
        {
            SceneLoader.Instance.LoadMainMenu();
        }

        public void LoadGame()
        {
            Destroy(GameManager.Instance.gameObject);
            Destroy(EnemyManager.Instance.gameObject);
            SceneLoader.Instance.LoadGame();
        }
    }
}