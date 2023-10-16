using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Actors
{
    public class ActorGameState : MonoBehaviour
    {
        [SerializeField] private GameState state;
        private ActorComponent[] _actorComponents;

        public void Construct(GameState newState)
        {
            state = newState;
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
        
        private void Start()
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
            foreach (var component in _actorComponents)
            {
                component.enabled = isActive;
            }
        }
        
        private void ResetActorComponents()
        {
            foreach (var component in _actorComponents)
            {
                component.Reset();
            }
        }
    }
}