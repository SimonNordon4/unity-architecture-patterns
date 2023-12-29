using System;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameObjectComponent.GameplayComponents;
using GameObjectComponent.GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Pools
{
    public class ActorPool : GameplayComponent
    {
        [field:SerializeField] public ActorFactory factory { get; private set; }
        private readonly Dictionary<ActorDefinition, Queue<PoolableActor>> _pools = new();
        public event Action<PoolableActor> OnEnemySpawned;
        public event Action<PoolableActor> OnEnemyDespawned;

        public PoolableActor Get(ActorDefinition definition, Vector3 position, bool startActive = true)
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
                actor.Construct(this);
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }
            else
            {
                actor = queue.Dequeue();
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }
            
            OnEnemySpawned?.Invoke(actor);
            
            return actor;
        }

        public void Return(PoolableActor poolableActor, ActorDefinition definition)
        {
            poolableActor.gameObject.SetActive(false);
            
            OnEnemyDespawned?.Invoke(poolableActor);
            
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
