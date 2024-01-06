using GameObjectComponent.Definitions;
using Pools;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Actor
{
    public class PoolableActor : GameplayComponent
    {
        private ActorPool _pool;
        private ActorDefinition _definition;
        
        public UnityEvent onActorGet = new();
        public UnityEvent onActorReturn = new();

        public void Construct(ActorPool pool, ActorDefinition definition)
        {
            _pool = pool;
            _definition = definition;
        }

        public void Return()
        {
            _pool.Return(this, _definition);
        }
        
        public bool TryGetComponentDeep<T>(out T component) where T : class
        {
            component = GetComponent<T>() ?? GetComponentInChildren<T>();
            return component != null;
        }
    }
}