using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace GameObjectComponent.Game
{
    public class GameState : ScriptableObject
    {
        public GameStateEnum currentState { get; private set; } = GameStateEnum.Idle;

        public event Action OnGameStart;
        public event Action OnGamePause;
        public event Action OnGameResume;
        public event Action OnGameWon;
        public event Action OnGameLost;
        public event Action OnGameQuit;

        private void OnEnable()
        {
            QuitGame();
        }
        public void StartNewGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            OnGameStart?.Invoke();
        }
        public void PauseGame()
        {
            currentState = GameStateEnum.Paused;
            GameTime.timeScale = 0f;
            OnGamePause?.Invoke();
        }
        public void ResumeGame()
        {
            currentState = GameStateEnum.Active;
            GameTime.timeScale = 1f;
            OnGameResume?.Invoke();
        }
        public void WinGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameWon?.Invoke();
        }
        public void GameOver()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameLost?.Invoke();
        }
        public void QuitGame()
        {
            currentState = GameStateEnum.Idle;
            GameTime.timeScale = 0f;
            OnGameQuit?.Invoke();
        }
        private void OnDestroy()
        {
            OnGameStart = null;
            OnGamePause = null;
            OnGameResume = null;
            OnGameWon = null;
            OnGameLost = null;
            OnGameQuit = null;
        }
    }
    
    public enum GameStateEnum
    {
        Idle,
        Active,
        Paused,
    }
    
        
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameState))]
    public class GameStateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var gameState = (GameState)target;
            
            // Show grayed out current state
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("Current State", gameState.currentState);
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Start New Game"))
            {
                gameState.StartNewGame();
            }
            if (GUILayout.Button("Pause Game"))
            {
                gameState.PauseGame();
            }
            if (GUILayout.Button("Resume Game"))
            {
                gameState.ResumeGame();
            }
            if (GUILayout.Button("Win Game"))
            {
                gameState.WinGame();
            }
            if (GUILayout.Button("Game Over"))
            {
                gameState.GameOver();
            }
            if (GUILayout.Button("Quit Game"))
            {
                gameState.QuitGame();
            }
        }
    }
#endif

}