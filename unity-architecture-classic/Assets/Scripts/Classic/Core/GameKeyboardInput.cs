using System;
using Classic.UI;
using UnityEngine;

namespace Classic.Core
{
    public class GameKeyboardInput : MonoBehaviour
    {
        [SerializeField] private KeyCode pauseKey = KeyCode.F;
        [SerializeField] private GameState gameState;
        [SerializeField] private UIState uiState;

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                switch (gameState.currentState)
                {
                    case GameStateEnum.Active:
                        gameState.PauseGame();
                        uiState.GoToPauseMenu();
                        break;
                    case GameStateEnum.Paused:
                        gameState.ResumeGame();
                        uiState.GoToHud();
                        break;
                    case GameStateEnum.Idle:
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnValidate()
        {
            if (gameState == null)
            {
                gameState = FindObjectsByType<GameState>(FindObjectsSortMode.None)[0];
            }
        }
    }
}