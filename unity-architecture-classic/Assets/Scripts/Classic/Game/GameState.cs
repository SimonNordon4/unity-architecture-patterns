using System;
using UnityEngine;


namespace Classic.Game
{
    /// <summary>
    /// GameState keeps track of the current state of the game, and notifies listeners when the state changes.
    /// </summary>
    public class GameState : MonoBehaviour
    {
        [SerializeField] private bool startActive = false;
        
        public GameStateEnum currentState { get; private set; } = GameStateEnum.Idle;

        public event Action<bool> OnChanged;
        public event Action OnGameStart;
        public event Action OnGamePause;
        public event Action OnGameResume;
        public event Action OnGameWon;
        public event Action OnGameLost;
        public event Action OnGameQuit;

        public bool isGameActive => currentState == GameStateEnum.Active;

        private void Start()
        {
            if (startActive)
            {
                StartNewGame();
            }
            else
            {
                QuitGame();
            }
        }

        public void StartNewGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            OnGameStart?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }
        
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            GameTime.timeScale = 0f;
            OnGamePause?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }
        
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            OnGameResume?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }

        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameWon?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }
        
        public void GameOver()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameLost?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }

        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameQuit?.Invoke();
            OnChanged?.Invoke(isGameActive);
        }

        private void OnDestroy()
        {
            OnChanged = null;
            OnGameStart = null;
            OnGamePause = null;
            OnGameResume = null;
            OnGameWon = null;
            OnGameLost = null;
            OnGameQuit = null;
        }
    }
    
    public enum GameStateEnum
    {
        Idle,
        Active,
        Paused,
    }
}