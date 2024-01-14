using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameObjectComponent.Definitions;
using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using UnityEngine;

namespace Pools
{
    public class ActorPool : GameplayComponent
    {
        [field:SerializeField] public ActorFactory factory { get; private set; }
        private readonly Dictionary<ActorDefinition, Queue<PoolableActor>> _inactivePools = new();
        private readonly List<PoolableActor> _activeActors = new();
        public event Action<PoolableActor> OnActorGet;
        public event Action<PoolableActor> OnActorReturn;

        public PoolableActor Get([DisallowNull]ActorDefinition definition, Vector3 position, bool startActive = true)
        {
            if (!_inactivePools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<PoolableActor>();
                _inactivePools.Add(definition, queue);
            }
            
            PoolableActor actor = null;

            if (queue.Count == 0)
            {
                actor = CreateActor(definition, position);
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
            _activeActors.Add(actor);
            actor.onActorGet?.Invoke();
            return actor;
        }

        public void Return(PoolableActor poolableActor, ActorDefinition definition)
        {
            poolableActor.onActorReturn?.Invoke();
            _activeActors.Remove(poolableActor);
            
            poolableActor.gameObject.SetActive(false);
            
            OnActorReturn?.Invoke(poolableActor);
            
            // Add the enemy to the queue
            _inactivePools[definition].Enqueue(poolableActor);
        }

        private PoolableActor CreateActor(ActorDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);
            return newEnemy;
        }

        public void ReturnAllActiveActors()
        {
            for (var i = _activeActors.Count - 1; i >= 0; i--)
            {
                _activeActors[i].Return();
            }
        }

        public void ResetAllPools()
        {
            ReturnAllActiveActors();
            foreach (var pool in _inactivePools)
            {
                foreach (var actor in pool.Value)
                {
                    Destroy(actor.gameObject);
                }
            }
            
            _inactivePools.Clear();
            OnActorReturn = null;
        }
        
        public override void OnGameEnd()
        {
            ResetAllPools();
        }

    }
}
