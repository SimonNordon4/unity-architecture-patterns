using GameplayComponents.Combat;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Debugging
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

        }
    }
}