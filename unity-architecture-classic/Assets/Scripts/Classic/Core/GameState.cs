using UnityEngine;
using UnityEngine.Events;

namespace Classic.Core
{
    public class GameState : MonoBehaviour
    {
        public GameStateEnum currentState = GameStateEnum.Idle;
        
        public UnityEvent onStateChanged { get; } = new();
        public UnityEvent onGameStart { get; } = new();
        public UnityEvent onGamePause { get; }= new();
        public UnityEvent onGameResume { get; }= new();
        public UnityEvent onGameWon { get; } = new();
        public UnityEvent onGameLost { get; } = new();
        public UnityEvent onGameQuit { get; } = new();

        public void StartNewGame()
        {
            currentState = GameStateEnum.Active;
            onGameStart.Invoke();
            onStateChanged.Invoke();
        }
        
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            onGamePause.Invoke();
            onStateChanged.Invoke();
        }
        
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            onGameResume.Invoke();
            onStateChanged.Invoke();
        }

        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            onGameWon.Invoke();
            onStateChanged.Invoke();
        }
        
        public void LoseGame()
        {
            currentState = GameStateEnum.Idle;
            onGameLost.Invoke();
            onStateChanged.Invoke();
        }
        
        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
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