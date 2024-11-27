using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIWinMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            timerText.text = $"Time Alive: {Mathf.FloorToInt(GameManager.Instance.roundTime / 60):00}:{Mathf.FloorToInt(GameManager.Instance.roundTime % 60):00}"; 
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
            GameManager.Instance.LoadMainMenu();
        }

        public void LoadGame()
        {
            Destroy(GameManager.Instance.gameObject);
            Destroy(EnemyManager.Instance.gameObject);
            GameManager.Instance.LoadGame();
        }
    }
}