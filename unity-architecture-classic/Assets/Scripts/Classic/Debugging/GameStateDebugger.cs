using Classic.Game;
using UnityEngine;

namespace Classic.Debugging
{
    public class GameStateDebugger : DebugComponent
    {
        [SerializeField] private GameState state;
        
        private void OnEnable()
        {
            state.OnChanged += OnStateChanged;
        }

        private void OnStateChanged(bool obj)
        {
            Print($"Game State Changed to {state.currentState}");
        }   
    }
}