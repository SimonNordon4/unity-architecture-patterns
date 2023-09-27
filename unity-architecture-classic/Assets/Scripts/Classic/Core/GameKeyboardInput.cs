using System;
using UnityEngine;

namespace Classic.Core
{
    public class GameKeyboardInput : MonoBehaviour
    {
        [SerializeField] private KeyCode pauseKey = KeyCode.F;
        
        [SerializeField] private GameState gameState;

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                switch (gameState.currentState)
                {
                    case GameStateEnum.Active:
                        gameState.PauseGame();
                        break;
                    case GameStateEnum.Paused:
                        gameState.ResumeGame();
                        break;
                    case GameStateEnum.Idle:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}