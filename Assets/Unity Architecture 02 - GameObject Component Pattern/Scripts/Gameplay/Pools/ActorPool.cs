using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(ActorFactory))]
    public class ActorPool : MonoBehaviour
    {
        [SerializeField]
        private ActorFactory actorFactory;

        public readonly Queue<PoolableActor> InactivePool = new();
        public readonly List<PoolableActor> ActivePool = new();

        public UnityEvent<PoolableActor> OnActorGet = new();
        public UnityEvent<PoolableActor> OnActorReturn = new();

        private void Awake()
        {
            actorFactory = GetComponent<ActorFactory>();
        }

        public PoolableActor Get(Vector3 position, bool startActive = true)
        {
            PoolableActor actor = null;

            if (InactivePool.Count == 0)
            {
                actor = actorFactory.Create(position);
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
            ActivePool.Add(actor);
            actor.onActorGet?.Invoke();
            return actor;
        }

        public void Return(PoolableActor actor)
        {
            actor.gameObject.SetActive(false);
            InactivePool.Enqueue(actor);
            OnActorReturn?.Invoke(actor);
        }

        public void ReturnAllActiveActors()
        {
            for (var i = ActivePool.Count - 1; i >= 0; i--)
            {
                ActivePool[i].Return();
            }
        } 
    }
}
