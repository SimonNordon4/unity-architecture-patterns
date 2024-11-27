using System;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Actor
{
    [DefaultExecutionOrder(-10)]
    public class GameplayStateController : MonoBehaviour
    {
        [SerializeField] private GameState state;
        private HashSet<GameplayComponent> _gameplayComponents = new HashSet<GameplayComponent>();

        public void Construct(GameState newState)
        {
            state = newState;
        }
        
        private void Awake()
        {
            GetActorComponents();
        }
        
        private void OnEnable()
        {
            state.OnGameStart += OnGameStart;
            state.OnGamePause += DisableActorComponents;
            state.OnGameResume += EnableActorComponents;
            state.OnGameWon += OnGameEnd;
            state.OnGameLost += OnGameEnd;
            state.OnGameQuit += OnGameEnd;
        }

        private void OnDisable() 
        {
            state.OnGameStart -= OnGameStart;
            state.OnGamePause -= DisableActorComponents;
            state.OnGameResume -= EnableActorComponents;
            state.OnGameWon -= OnGameEnd;
            state.OnGameLost -= OnGameEnd;
            state.OnGameQuit -= OnGameEnd;
        }

        private void GetActorComponents()
        {
            var children = GetComponentsInChildren<GameplayComponent>();
            var siblings = GetComponents<GameplayComponent>();
            _gameplayComponents = new HashSet<GameplayComponent>(children);
            
            foreach (var sibling in siblings)
            {
                _gameplayComponents.Add(sibling);
            }
        }

        public void OnGameStart()
        {
            EnableActorComponents();
            foreach (var component in _gameplayComponents)
            {
                component.OnGameStart();
            }
        }
        
        public void OnGameEnd()
        {
            foreach (var component in _gameplayComponents)
            {
                component.OnGameEnd();
            }
            DisableActorComponents();
        }

        public void EnableActorComponents()
        {
            foreach (var component in _gameplayComponents)
            {
                component.enabled = true;
            }
        }
        
        public void DisableActorComponents()
        {
            foreach (var component in _gameplayComponents)
            {
                component.enabled = false;
            }
        }
    }
}