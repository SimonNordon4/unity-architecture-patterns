using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(GameState))]
    public class GameKeyboardInput : MonoBehaviour
    {
        [SerializeField] private KeyCode pauseKey = KeyCode.F;
        [SerializeField] private KeyCode debugKey = KeyCode.G;
        private GameState _gameState;

        [SerializeField] private GameObject graphy;


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

            if (Input.GetKeyDown(debugKey))
            {
                graphy.SetActive(!graphy.activeSelf);
            }
            
            
        }
    }
}