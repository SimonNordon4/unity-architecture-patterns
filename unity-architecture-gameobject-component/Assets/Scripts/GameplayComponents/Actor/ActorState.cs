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
        
        public event Action OnResetComponents;
        public event Action OnEnableComponents;
        public event Action OnDisableComponents; 
        

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
            state.OnChanged += ToggleActorComponents;
            state.OnGameStart += ResetActorComponents;
        }

        private void OnDisable() 
        {
            state.OnChanged -= ToggleActorComponents;
            state.OnGameStart -= ResetActorComponents;
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

        private void ToggleActorComponents(bool isActive)
        {
            if (_gameplayComponents == null)
            {
                GetActorComponents();
            }
            foreach (var component in _gameplayComponents)
            {
                component.enabled = isActive;
            }
            
            if (isActive)
            {
                OnEnableComponents?.Invoke();
            }
            else
            {
                OnDisableComponents?.Invoke();
            }
        }
        
        private void ResetActorComponents()
        {
            foreach (var component in _gameplayComponents)
            {
                component.Reset();
            }
            
            OnResetComponents?.Invoke();
        }

        public void ResetActor()
        {
            ResetActorComponents();
        }
        
        public void DisableActorComponents()
        {
            ToggleActorComponents(false);
        }
        
        public void EnableActorComponents()
        {
            ToggleActorComponents(true);
        }
    }
}