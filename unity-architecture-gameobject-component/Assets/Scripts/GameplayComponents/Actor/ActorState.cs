using System;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Actor
{
    [DefaultExecutionOrder(-10)]
    public class ActorState : MonoBehaviour
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
            state.OnGameStart += InitializeComponents;
            state.OnGamePause += DisableActorComponents;
            state.OnGameResume += EnableActorComponents;
            state.OnGameWon += DeInitializeComponents;
            state.OnGameLost += DeInitializeComponents;
            state.OnGameQuit += DeInitializeComponents;
        }

        private void OnDisable() 
        {
            state.OnGameStart -= InitializeComponents;
            state.OnGamePause -= DisableActorComponents;
            state.OnGameResume -= EnableActorComponents;
            state.OnGameWon -= DeInitializeComponents;
            state.OnGameLost -= DeInitializeComponents;
            state.OnGameQuit -= DeInitializeComponents;
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

        public void InitializeComponents()
        {
            EnableActorComponents();
            foreach (var component in _gameplayComponents)
            {
                component.Initialize();
            }
        }
        
        public void DeInitializeComponents()
        {
            foreach (var component in _gameplayComponents)
            {
                component.Deinitialize();
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