using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UnityArchitecture.GameObjectComponentPattern
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
            Debug.Log("Game Started");
            onGameStarted?.Invoke();
        }

        public void WinGame()
        {
            Debug.Log("Game Won");
            onGameWon?.Invoke();
        }

        public void LoseGame()
        {
            Debug.Log("Game Lost");
            onGameLost?.Invoke();
        }

        public void PauseGame()
        {
            Debug.Log("Game Paused");
            onGamePaused?.Invoke();
            IsPaused = !IsPaused;
        }

        public void ResumeGame()
        {
            IsPaused = !IsPaused;   
            Debug.Log("Game Resumed");
            onGameResumed?.Invoke();
        }
    }
}
