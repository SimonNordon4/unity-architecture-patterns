using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.Debugging
{
    public class GameStateDebugger : DebugComponent
    {
        [SerializeField] private GameState state;
        
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
}