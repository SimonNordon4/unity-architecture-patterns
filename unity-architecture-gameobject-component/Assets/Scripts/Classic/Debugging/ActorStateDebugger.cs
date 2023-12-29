using Classic.Actors;
using UnityEngine;

namespace Classic.Debugging
{
    [RequireComponent(typeof(ActorState))]
    public class ActorStateDebugger : DebugComponent
    {
        private ActorState _actorState;
        
        private void Awake()
        {
            _actorState = GetComponent<ActorState>();
        }
        
        private void OnEnable()
        {
            _actorState.OnResetComponents += () => Print($"Actor Components Reset on {gameObject.name}");
            _actorState.OnEnableComponents += () => Print($"Actor Components Enabled on {gameObject.name}");
            _actorState.OnDisableComponents += () => Print($"Actor Components Disabled on {gameObject.name}");
        }
    }
}