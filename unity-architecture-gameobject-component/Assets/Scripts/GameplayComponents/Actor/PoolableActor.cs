using GameObjectComponent.Definitions;
using GameObjectComponent.Pools;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Actor
{
    public class PoolableActor : GameplayComponent
    {
        [SerializeField]private ActorDefinition definition;
        private ActorPool _pool;
        
        public void Construct(ActorPool pool)
        {
            _pool = pool;
        }
        
        public override void Reset()
        {
            _pool.Return(this, definition);
        }
    }
}