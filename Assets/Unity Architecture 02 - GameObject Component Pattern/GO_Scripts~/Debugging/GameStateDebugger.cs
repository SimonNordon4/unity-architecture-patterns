using GameObjectComponent.Game;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameObjectComponent.Debugging
{
    public class GameStateDebugger : DebugComponent
    {
        [field:SerializeField] public GameState state { get; private set; }
        
        private void OnEnable()
        {
            state.OnGameStart += () => Print("Game Started");
            state.OnGamePause += () => Print("Game Paused");
            state.OnGameResume += () => Print("Game Resumed");
            state.OnGameWon += () => Print("Game Won");
            state.OnGameLost += () => Print("Game Lost");
            state.OnGameQuit += () => Print("Game Quit");
            
        }

        private void OnStateChanged(bool obj)
        {
            Print($"Game State Changed to {state.currentState}");
        }   
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(GameStateDebugger))]
    public class GameStateDebuggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var gameStateDebugger = (GameStateDebugger) target;
            GUILayout.Label(gameStateDebugger.state.currentState.ToString());
            
            if (GUILayout.Button("Start New Game"))
            {
                gameStateDebugger.state.StartNewGame();
            }
            
            if (GUILayout.Button("Pause Game"))
            {
                gameStateDebugger.state.PauseGame();
            }
            
            if (GUILayout.Button("Resume Game"))
            {
                gameStateDebugger.state.ResumeGame();
            }
            
            if (GUILayout.Button("Win Game"))
            {
                gameStateDebugger.state.WinGame();
            }
            
            if (GUILayout.Button("Lose Game"))
            {
                gameStateDebugger.state.GameOver();
            }
            
            if (GUILayout.Button("Quit Game"))
            {
                gameStateDebugger.state.QuitGame();
            }
        }
    }
    #endif
}