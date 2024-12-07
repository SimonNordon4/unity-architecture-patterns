using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class GameState : MonoBehaviour
    {
        public bool IsPaused { get; private set; }
        
        public UnityEvent onGameStarted = new();
        public UnityEvent onGameWon = new();
        public UnityEvent onGameLost = new();
        public UnityEvent onGamePaused = new();
        public UnityEvent onGameResumed = new();

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            IsPaused = false;
            onGameStarted?.Invoke();
        }

        public void WinGame()
        {
            onGameWon?.Invoke();
        }

        public void LoseGame()
        {
            onGameLost?.Invoke();
        }

        public void PauseGame()
        {
            onGamePaused?.Invoke();
            IsPaused = !IsPaused;
        }

        public void ResumeGame()
        {
            IsPaused = !IsPaused;   
            onGameResumed?.Invoke();
        }
    }
}
