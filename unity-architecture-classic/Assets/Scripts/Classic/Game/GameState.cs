using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    public class GameState : MonoBehaviour
    {
        public GameStateEnum currentState { get; private set; } = GameStateEnum.Idle;
        
        public UnityEvent onStateChanged { get; } = new();
        public UnityEvent onGameStart { get; } = new();
        public UnityEvent onGamePause { get; } = new();
        public UnityEvent onGameResume {get;} = new();
        public UnityEvent onGameWon {get;}  = new();
        public UnityEvent onGameLost {get;}  = new();
        public UnityEvent onGameQuit {get;}  = new();

        public bool isGameActive => currentState == GameStateEnum.Active;

        private void Start()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            onStateChanged.Invoke();
        }

        public void StartNewGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            onGameStart.Invoke();
            onStateChanged.Invoke();
        }
        
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            GameTime.timeScale = 0f;
            onGamePause.Invoke();
            onStateChanged.Invoke();
        }
        
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            onGameResume.Invoke();
            onStateChanged.Invoke();
        }

        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            onGameWon.Invoke();
            onStateChanged.Invoke();
        }
        
        public void LoseGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            onGameLost.Invoke();
            onStateChanged.Invoke();
        }

        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            onGameQuit.Invoke();
            onStateChanged.Invoke();
        }
    }
    
    public enum GameStateEnum
    {
        Idle,
        Active,
        Paused,
    }
}