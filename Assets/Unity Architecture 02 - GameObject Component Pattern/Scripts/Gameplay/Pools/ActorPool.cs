using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{

    public class ActorPool : MonoBehaviour
    {
        [SerializeField]
        private ActorFactory actorFactory;

        public override FactoryBase<PoolableActor> Factory 
        {
            get => actorFactory;
            protected set => actorFactory = (ActorFactory)value;
        }

        public event Action<PoolableActor> OnActorGet;
        public event Action<PoolableActor> OnActorReturn;

        private void Awake()
        {
            actorFactory = GetComponent<ActorFactory>();
        }

        public PoolableActor Get(Vector3 position, bool startActive = true)
        {
            
            PoolableActor actor = null;

            if (InactivePool.Count == 0)
            {
                actor = Factory.Create(position, false);
                actor.Construct(this);
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }
            else
            {
                actor = InactivePool.Dequeue();
                actor.transform.position = position;
                actor.gameObject.SetActive(startActive);
            }

            OnActorGet?.Invoke(actor);
            ActivePool.Enqueue(actor);
            actor.onActorGet?.Invoke();
            return actor;
        }

        public void ReturnAllActiveActors()
        {
            for (var i = _activeActors.Count - 1; i >= 0; i--)
            {
                _activeActors[i].Return();
            }
        }

        // destroy all actors in the pool
        public void FlushPool()
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
    

    }
}
