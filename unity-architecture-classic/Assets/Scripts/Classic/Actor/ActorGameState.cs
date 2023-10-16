using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actor
{
    public class ActorGameState : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private List<MonoBehaviour> behaviours = new();

        public UnityEvent onGameStart = new();

        private void OnEnable()
        {
            state.OnChanged+=(GameStateChanged);
            state.OnGameStart+=(GameStarted);            
        }

        private void GameStarted()
        {
            onGameStart.Invoke();
        }

        private void GameStateChanged(bool isActive)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = isActive;
            }
        }
    }
}