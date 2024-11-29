using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class GameState : MonoBehaviour
    {
        public UnityEvent OnGameStarted = new();
        public UnityEvent OnGameWon = new();
        public UnityEvent OnGameLost = new();
        public UnityEvent OnGamePaused = new();
        public UnityEvent OnGameResumed = new();

        public void StartGame()
        {
            Debug.Log("Game Started");
            OnGameStarted?.Invoke();
        }

        public void WinGame()
        {
            Debug.Log("Game Won");
            OnGameWon?.Invoke();
        }

        public void LoseGame()
        {
            Debug.Log("Game Lost");
            OnGameLost?.Invoke();
        }

        public void PauseGame()
        {
            Debug.Log("Game Paused");
            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            Debug.Log("Game Resumed");
            OnGameResumed?.Invoke();
        }
    }
}
