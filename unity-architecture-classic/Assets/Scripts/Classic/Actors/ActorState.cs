using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actors
{
    [DefaultExecutionOrder(10)]
    public class ActorState : MonoBehaviour
    {
        [SerializeField] private GameState state;
        private ActorComponent[] _actorComponents;

        public void Construct(GameState newState)
        {
            state = newState;
        }

        private void OnEnable()
        {
            GetActorComponents();
            state.OnChanged += ToggleActorComponents;
            state.OnGameStart += ResetActorComponents;
        }

        private void OnDisable() 
        {
            state.OnChanged -= ToggleActorComponents;
            state.OnGameStart -= ResetActorComponents;
        }
        
        private void Awake()
        {
            GetActorComponents();
        }

        private void GetActorComponents()
        {
            var children = GetComponentsInChildren<ActorComponent>();
            var siblings = GetComponents<ActorComponent>();
            _actorComponents = new ActorComponent[children.Length + siblings.Length];
            children.CopyTo(_actorComponents, 0);
            siblings.CopyTo(_actorComponents, children.Length);
        }

        private void ToggleActorComponents(bool isActive)
        {
            if (_actorComponents == null)
            {
                GetActorComponents();
            }
            foreach (var component in _actorComponents)
            {
                component.enabled = isActive;
            }
        }
        
        private void ResetActorComponents()
        {
            Debug.Log($"Resetting actor components for GameObject: {gameObject.name}", this);
            foreach (var component in _actorComponents)
            {
                component.Reset();
            }
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