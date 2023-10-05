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