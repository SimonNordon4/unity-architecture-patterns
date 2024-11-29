using System;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class GameState : MonoBehaviour
    {
        public Action OnGameStarted = delegate { };
        public Action OnGameWon = delegate { };
        public Action OnGameLost = delegate { };
        public Action OnGamePaused = delegate { };
        public Action OnGameResumed = delegate { };

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
