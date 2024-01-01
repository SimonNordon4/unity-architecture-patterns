using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameObjectComponent.Definitions;
using GameplayComponents;
using GameplayComponents.Actor;
using UnityEngine;

namespace Pools
{
    public class ActorPool : GameplayComponent
    {
        [field:SerializeField] public ActorFactory factory { get; private set; }
        private readonly Dictionary<ActorDefinition, Queue<PoolableActor>> _pools = new();
        public event Action<PoolableActor> OnActorGet;
        public event Action<PoolableActor> OnActorReturn;

        public PoolableActor Get([DisallowNull]ActorDefinition definition, Vector3 position, bool startActive = true)
        {
            if (!_pools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<PoolableActor>();
                _pools.Add(definition, queue);
            }
            
            PoolableActor actor = null;

            if (queue.Count == 0)
            {
                actor = CreateEnemy(definition, position);
                actor.Construct(this, definition);
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }
            else
            {
                actor = queue.Dequeue();
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }
            
            OnActorGet?.Invoke(actor);
            
            return actor;
        }

        public void Return(PoolableActor poolableActor, ActorDefinition definition)
        {
            poolableActor.gameObject.SetActive(false);
            
            OnActorReturn?.Invoke(poolableActor);
            
            // Add the enemy to the queue
            _pools[definition].Enqueue(poolableActor);
        }

        private PoolableActor CreateEnemy(ActorDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);
            return newEnemy;
        }

    }
}
