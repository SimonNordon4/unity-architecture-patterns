using System;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(GameState))]
    public class GameKeyboardInput : MonoBehaviour
    {
        [SerializeField] private KeyCode pauseKey = KeyCode.F;
        private GameState _gameState;

        private void Awake()
        {
            _gameState = GetComponent<GameState>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey) && _gameState.IsPaused)
            {
                _gameState.ResumeGame();
            }
            else if (Input.GetKeyDown(pauseKey) && !_gameState.IsPaused)
            {
                _gameState.PauseGame();
            }
        }
    }
}