using System;
using UnityEngine;


namespace GameObjectComponent.Game
{
    /// <summary>
    /// GameState keeps track of the current state of the game, and notifies listeners when the state changes.
    /// </summary>
    public class GameState : MonoBehaviour
    {
        [SerializeField] private bool startActive = false;
        
        public GameStateEnum currentState { get; private set; } = GameStateEnum.Idle;

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
        }
        
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            GameTime.timeScale = 0f;
            OnGamePause?.Invoke();
        }
        
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            OnGameResume?.Invoke();
        }

        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameWon?.Invoke();
        }
        
        public void GameOver()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameLost?.Invoke();
        }

        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameQuit?.Invoke();
        }

        private void OnDestroy()
        {
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