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
            OnGameStarted?.Invoke();
        }

        public void WinGame()
        {
            OnGameWon?.Invoke();
        }

        public void LoseGame()
        {
            OnGameLost?.Invoke();
        }

        public void PauseGame()
        {
            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            OnGameResumed?.Invoke();
        }
    }
}
