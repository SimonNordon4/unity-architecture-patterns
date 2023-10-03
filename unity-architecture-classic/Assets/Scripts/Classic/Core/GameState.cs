using UnityEngine;
using UnityEngine.Events;

namespace Classic.Core
{
    public class GameState : MonoBehaviour
    {
        public GameStateEnum currentState { get; private set; } = GameStateEnum.Idle;
        
        public UnityEvent onStateChanged { get; } = new();
        public UnityEvent onGameStart = new();
        public UnityEvent onGamePause = new();
        public UnityEvent onGameResume = new();
        public UnityEvent onGameWon  = new();
        public UnityEvent onGameLost  = new();
        public UnityEvent onGameQuit  = new();

        private void Start()
        {
            currentState = GameStateEnum.Idle;
            onStateChanged.Invoke();
            GameTime.timeScale = 0f;
        }

        public void StartNewGame()
        {
            currentState = GameStateEnum.Active;
            onGameStart.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 1f;
        }
        
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            onGamePause.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            onGameResume.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 1f;
        }

        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            onGameWon.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 0f;
        }
        
        public void LoseGame()
        {
            currentState = GameStateEnum.Idle;
            onGameLost.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 0f;
        }

        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
            onGameQuit.Invoke();
            onStateChanged.Invoke();
            GameTime.timeScale = 0f;
        }
    }
    
    public enum GameStateEnum
    {
        Idle,
        Active,
        Paused,
    }
}